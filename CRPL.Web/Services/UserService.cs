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
using CRPL.Data.Applications;
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

        return new UserAccountStatusModel
        {
            UserAccount = Mapper.Map<UserAccountViewModel>(user),
            PartialFields = user.Status != UserAccount.AccountStatus.Complete ? getPartials(user) : null
        };
    }

    public Task<UserWallet> GetWallet(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsUniquePhoneNumber(string phoneNumber)
    {
        Logger.LogInformation("Checking is phone number '{Phone}' exists", phoneNumber);
        return !(await Context.UserAccounts.AnyAsync(x => x.PhoneNumber == phoneNumber));
    }

    public async Task<bool> IsUniqueEmail(string email)
    {
        Logger.LogInformation("Checking is email '{Email}' exists", email);
        return !(await Context.UserAccounts.AnyAsync(x => x.Email == email));
    }

    public bool AreUsersReal(List<string> userAddresses)
    {
        Logger.LogInformation("are these users real? {Users}", string.Join(",", userAddresses));
        var userAccounts = Context.UserAccounts.Where(x => userAddresses.Select(a => a.ToLower()).Contains(x.Wallet.PublicAddress.ToLower())).ToList();
        return userAccounts.Count == userAddresses.Count;
    }

    public async Task<List<UserAccountMinimalViewModel>> SearchUsers(string address)
    {
        Logger.LogInformation("Searching for users with address like: {Address}", address);
        return await Context.UserAccounts.Where(x => x.Wallet.PublicAddress.Contains(address.ToLower())).Select(x => Mapper.Map<UserAccountMinimalViewModel>(x)).ToListAsync();
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
            if (prop != null && property.Name != "DateOfBirth")
            {
                typeof(UserAccount).GetProperty(property.Name)?.SetValue(user, prop);
            }
        }

        if (accountInputModel.DateOfBirth != null)
        {
            user.DateOfBirth = new UserAccount.DOB()
            {
                Year = accountInputModel.DateOfBirth.Year,
                Month = accountInputModel.DateOfBirth.Month,
                Day = accountInputModel.DateOfBirth.Day
            };
        }

        if (user.Status == UserAccount.AccountStatus.Created) user.Status = UserAccount.AccountStatus.Incomplete;
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
        var ignoredProperties = new List<string> { "Wallet", "UserWorks", "AuthenticationToken", "Applications" };
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
        Logger.LogInformation("Checking if a user has completed sign up, {Id}", userAccount.Id);
        var ignoredProperties = new List<string> { "Email", "PhoneNumber", "Wallet", "UserWorks", "AuthenticationToken", "Applications" };
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

    public void AssignToApplication(string address, Guid applicationId)
    {
        Logger.LogInformation("Assigning {Address} to {Id}", address, applicationId);
        
        var user = Context.UserAccounts.Include(x => x.Applications).FirstOrDefault(x => x.Wallet.PublicAddress == address);
        if (user == null) throw new UserNotFoundException(address);

        Context.UserAccounts.Update(user);

        if (user.Applications == null) user.Applications = new List<UserApplication>();

        if (user.Applications.FirstOrDefault(x => x.ApplicationId == applicationId) != null)
        {
            Logger.LogInformation("User already assigned, skipping...");
            return;
        }

        user.Applications.Add(new UserApplication()
        {
            ApplicationId = applicationId
        });

        Context.SaveChanges();
    }

    // when no account exists create and save
    public async Task<string> FetchNonce(string walletAddress)
    {
        Logger.LogInformation("Fetching {Address}'s nonce", walletAddress);

        var user = await Context.UserAccounts.Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Wallet.PublicAddress == walletAddress);

        // if new user
        if (user == null)
        {
            Logger.LogInformation("New user {Address} found when fetching nonce", walletAddress);
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
        Logger.LogInformation("Authenticating the signature: {Sig} for {Address}", authenticateInputModel.Signature, authenticateInputModel.WalletAddress);

        // null check
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.Wallet.PublicAddress == authenticateInputModel.WalletAddress);
        if (user == null) throw new UserNotFoundException(authenticateInputModel.WalletAddress);

        var message = $"Signing a unique nonce {user.Wallet.Nonce}";

        // verifying signature
        var verifiedAddress = new EthereumMessageSigner().EncodeUTF8AndEcRecover(message, authenticateInputModel.Signature).ToLower();

        Logger.LogInformation("verified address {VAddress} compared to {Address}", verifiedAddress, user.Wallet.PublicAddress);

        // if the wallet owner is not the signer
        if (!string.Equals(verifiedAddress, user.Wallet.PublicAddress, StringComparison.Ordinal))
            throw new InvalidSignatureException(authenticateInputModel.Signature, authenticateInputModel.WalletAddress);

        user.AuthenticationToken = generateToken(user, 30);

        // refresh nonce once used for auth
        user.Wallet.Nonce = generateNonce();
        await Context.SaveChangesAsync();

        return new AuthenticateResult
        {
            Token = user.AuthenticationToken,
            Log = $"Verified user by {message}",
            Account = Mapper.Map<UserAccountViewModel>(user)
        };
    }

    public async Task<UserAccountViewModel> Authenticate(string token)
    {
        Logger.LogInformation("Authenticating a token {Token}", token);
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.AuthenticationToken == token);
        if (user == null) throw new InvalidAuthenticationException(token);
        return Mapper.Map<UserAccountViewModel>(user);
    }

    private string generateToken(UserAccount user, int days)
    {
        Logger.LogInformation("Generating auth token for {Id}, lasting {Days} days", user.Wallet.PublicAddress, days);

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
        Logger.LogInformation("Revoking authentication to {Token}", token);

        // null check
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.AuthenticationToken == token);
        if (user == null) throw new UserNotFoundException();

        Context.UserAccounts.Update(user);
        user.AuthenticationToken = null;
        await Context.SaveChangesAsync();
    }

    public async Task<bool> isShareholder(string address, string rightId)
    {
        Logger.LogInformation("Checking if shareholder {Address}", address);

        // null check
        var user = await Context.UserAccounts
            .Include(x => x.Wallet)
            .Include(x => x.UserWorks).ThenInclude(x => x.RegisteredWork)
            .FirstOrDefaultAsync(x => x.Wallet.PublicAddress == address);
        if (user == null) throw new UserNotFoundException();

        return user.UserWorks.Any(x => x.RegisteredWork.RightId == rightId);
    }
}