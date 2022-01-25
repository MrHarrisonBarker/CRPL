using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Data.Account.ViewModels;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nethereum.Signer;

namespace CRPL.Web.Services;

// all methods effecting user accounts and user wallets
public class UserService : IUserService
{
    private readonly ILogger<UserService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly AppSettings Options;

    public UserService(ILogger<UserService> logger, ApplicationContext context, IMapper mapper, IOptions<AppSettings> options)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        Options = options.Value;
    }

    public async Task<UserAccountStatusModel> GetAccount(Guid id)
    {
        Logger.LogInformation("Getting account {Id}", id);

        // null check
        var user = await Context.UserAccounts.Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) throw new UserNotFoundException();

        return new UserAccountStatusModel()
        {
            UserAccount = Mapper.Map<UserAccountViewModel>(user),
            PartialFields = user.Status != UserAccount.AccountStatus.Complete ? getPartials(user) : null
        };
    }

    public Task<UserWallet> GetWallet(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public async Task<UserAccountStatusModel> UpdateAccount(Guid accountId, AccountInputModel accountInputModel)
    {
        Logger.LogInformation("Updating account {Id}", accountId);

        // null check
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.Id == accountId);
        if (user == null) throw new UserNotFoundException();

        // marks for updating
        Context.UserAccounts.Update(user);

        // update properties present in the AccountInputModel
        foreach (var property in typeof(AccountInputModel).GetProperties())
        {
            var prop = property.GetValue(accountInputModel);
            if (prop != null)
            {
                typeof(UserAccount).GetProperty(property.Name)?.SetValue(user, prop);
            }
        }

        if (isComplete(user)) user.Status = UserAccount.AccountStatus.Complete;

        await Context.SaveChangesAsync();

        return new UserAccountStatusModel
        {
            UserAccount = Mapper.Map<UserAccount, UserAccountViewModel>(user),
            PartialFields = getPartials(user)
        };
    }

    private List<PartialField> getPartials(UserAccount userAccount)
    {
        var ignoredProperties = new List<string> { "Wallet", "RegisteredWorks" };
        var partials = new List<PartialField>();

        foreach (var property in typeof(UserAccount).GetProperties())
        {
            if (property.GetValue(userAccount) == null && !ignoredProperties.Contains(property.Name))
            {
                partials.Add(new PartialField
                {
                    Field = property.Name,
                    Type = property.PropertyType.Name
                });
            }
        }

        return partials;
    }

    private bool isComplete(UserAccount userAccount)
    {
        var ignoredProperties = new List<string> { "Email", "PhoneNumber", "Wallet", "RegisteredWorks" };
        var hasContact = 0;
        // checks each property is not null
        // user only needs one form of contact: email or phone
        foreach (var property in typeof(UserAccount).GetProperties())
        {
            var val = property.GetValue(userAccount);
            if (val == null && !ignoredProperties.Contains(property.Name)) return false;
            if ((property.Name == "Email" || property.Name == "PhoneNumber") && val != null) hasContact++;
        }

        return hasContact != 0;
    }

    public Task<string> UpdateWallet(Guid accountId, WalletInputModel walletInputModel)
    {
        throw new NotImplementedException();
    }

    // when no account exists create and save
    public async Task<string> FetchNonce(string walletAddress)
    {
        Logger.LogInformation("Fetching {Id}'s nonce", walletAddress);

        var user = await Context.UserAccounts.Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Wallet.PublicAddress == walletAddress);

        // if new user
        if (user == null)
        {
            Logger.LogInformation("New user found when fetching nonce");
            user = new UserAccount
            {
                Wallet = new UserWallet
                {
                    PublicAddress = walletAddress.ToLower()
                }
            };
            await Context.UserAccounts.AddAsync(user);
        }

        user.Wallet.Nonce = generateNonce();

        await Context.SaveChangesAsync();

        return user.Wallet.Nonce;
    }

    private string generateNonce()
    {
        Logger.LogInformation("Generating new nonce");

        var arr = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(arr);
        return Convert.ToHexString(arr);
    }

    public async Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel)
    {
        Logger.LogInformation("Authenticating a users signature");

        // null check
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.Wallet.PublicAddress == authenticateInputModel.WalletAddress);
        if (user == null) throw new UserNotFoundException();

        var message = $"Signing a unique nonce {user.Wallet.Nonce}";

        // verifying signature
        var verifiedAddress = new EthereumMessageSigner().EncodeUTF8AndEcRecover(message, authenticateInputModel.Signature).ToLower();

        Logger.LogInformation("verified address {VAddress} compared to {Address}", verifiedAddress, user.Wallet.PublicAddress);

        // if the wallet owner is not the signer
        if (verifiedAddress != user.Wallet.PublicAddress) throw new InvalidSignature();

        user.AuthenticationToken = generateToken(user, 30);

        // refresh nonce once used for auth
        user.Wallet.Nonce = generateNonce();
        await Context.SaveChangesAsync();

        return new AuthenticateResult
        {
            Token = user.AuthenticationToken,
            Log = $"Verified user by {message}"
        };
    }

    public async Task Authenticate(string token)
    {
        // null check
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.AuthenticationToken == token);
        if (user == null) throw new UnauthorizedAccessException();
    }

    private string generateToken(UserAccount user, int days)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Options.EncryptionKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(days),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task RevokeAuthentication(string token)
    {
        // null check
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.AuthenticationToken == token);
        if (user == null) throw new UserNotFoundException();

        Context.UserAccounts.Update(user);
        user.AuthenticationToken = null;
        await Context.SaveChangesAsync();
    }
}