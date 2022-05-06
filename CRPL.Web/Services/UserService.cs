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

// A service creating, updating and interacting with user accounts
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

    // Get a user account with all missing fields
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

    // Check if the input phone number is already in the database
    public async Task<bool> IsUniquePhoneNumber(Guid id, string phoneNumber)
    {
        Logger.LogInformation("Checking is phone number '{Phone}' exists", phoneNumber);
        return !(await Context.UserAccounts.Where(x => x.Id != id).AnyAsync(x => x.PhoneNumber == phoneNumber));
    }

    // Check if the input email is already in the database
    public async Task<bool> IsUniqueEmail(Guid id, string email)
    {
        Logger.LogInformation("Checking is email '{Email}' exists", email);
        return !(await Context.UserAccounts.Where(x => x.Id != id).AnyAsync(x => x.Email == email));
    }

    // Check if all the users exist in the database
    public bool AreUsersReal(List<string> userAddresses)
    {
        Logger.LogInformation("are these users real? {Users}", string.Join(",", userAddresses));
        var userAccounts = Context.UserAccounts.Where(x => userAddresses.Select(a => a.ToLower()).Contains(x.Wallet.PublicAddress.ToLower())).ToList();
        // Number of users matches query
        return userAccounts.Count == userAddresses.Count;
    }

    // Search for a user based on wallet address
    public async Task<List<UserAccountMinimalViewModel>> SearchUsers(string address)
    {
        Logger.LogInformation("Searching for users with address like: {Address}", address);
        return await Context.UserAccounts.Where(x => x.Wallet.PublicAddress.Contains(address.ToLower())).Select(x => Mapper.Map<UserAccountMinimalViewModel>(x)).ToListAsync();
    }

    // Update a users data model based on input model
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

        // Update date of birth
        if (accountInputModel.DateOfBirth != null)
        {
            user.DateOfBirth = new UserAccount.DOB()
            {
                Year = accountInputModel.DateOfBirth.Year,
                Month = accountInputModel.DateOfBirth.Month,
                Day = accountInputModel.DateOfBirth.Day
            };
        }

        // Set status of user account if complete
        if (user.Status == UserAccount.AccountStatus.Created) user.Status = UserAccount.AccountStatus.Incomplete;
        user.Status = isComplete(user) ? UserAccount.AccountStatus.Complete : UserAccount.AccountStatus.Incomplete;

        await Context.SaveChangesAsync();

        return new UserAccountStatusModel
        {
            UserAccount = Mapper.Map<UserAccount, UserAccountViewModel>(user),
            PartialFields = getPartials(user)
        };
    }

    // Gets all fields without a value 
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

    // Returns true if all the required fields have been filled for a complete user
    private bool isComplete(UserAccount userAccount)
    {
        Logger.LogInformation("Checking if a user has completed sign up, {Id}", userAccount.Id);
        
        // All properties ignored from the null check
        var ignoredProperties = new List<string> { "Email", "PhoneNumber", "DialCode", "Wallet", "UserWorks", "AuthenticationToken", "Applications" };
        
        // checks each property is not null
        foreach (var property in typeof(UserAccount).GetProperties())
        {
            var val = property.GetValue(userAccount);
            if (val == null && !ignoredProperties.Contains(property.Name)) return false;
            if (val?.GetType() == typeof(string))
            {
                if (string.IsNullOrEmpty((string?)val) || string.IsNullOrWhiteSpace((string?)val)) return false;
            }
        }

        // Make sure the user has at least one form of contact
        if (userAccount.Email != null) return true;
        if (userAccount.PhoneNumber != null && userAccount.DialCode != null) return true;

        return false;
    }

    // Create a database relationship between an application and user, using wallet address
    public void AssignToApplication(string address, Guid applicationId)
    {
        Logger.LogInformation("Assigning {Address} to application {Id}", address, applicationId);

        var user = Context.UserAccounts.Include(x => x.Applications).FirstOrDefault(x => x.Wallet.PublicAddress == address);
        if (user == null) throw new UserNotFoundException(address);

        Context.UserAccounts.Update(user);

        AssignUserToApplication(user, applicationId);

        Context.SaveChanges();
    }

    // Create a database relationship between an application and user, using id
    public void AssignToApplication(Guid id, Guid applicationId)
    {
        Logger.LogInformation("Assigning {Id} to application {Id}", id, applicationId);

        var user = Context.UserAccounts.Include(x => x.Applications).FirstOrDefault(x => x.Id == id);
        if (user == null) throw new UserNotFoundException(id);

        Context.UserAccounts.Update(user);

        AssignUserToApplication(user, applicationId);

        Context.SaveChanges();
    }

    // Create a database relationship between an application and user
    private void AssignUserToApplication(UserAccount user, Guid applicationId)
    {
        if (user.Applications == null) user.Applications = new List<UserApplication>();

        if (user.Applications.FirstOrDefault(x => x.ApplicationId == applicationId) != null)
        {
            Logger.LogInformation("User already assigned, skipping...");
            return;
        }

        // Create relationship using junction table
        user.Applications.Add(new UserApplication()
        {
            ApplicationId = applicationId
        });
    }

    // First stage of authentication, If no user create a new user and generate a nonce
    public async Task<string> FetchNonce(string walletAddress)
    {
        Logger.LogInformation("Fetching {Address}'s nonce", walletAddress);

        var user = await Context.UserAccounts.Include(x => x.Wallet).FirstOrDefaultAsync(x => x.Wallet.PublicAddress == walletAddress);

        // if no user exists create a new one
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

    // Generate random 32 byte string (256 bit)
    private string generateNonce()
    {
        Logger.LogInformation("Generating new nonce");

        var arr = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(arr);
        return Convert.ToHexString(arr);
    }

    // Second step of authentication, Compare signature derived address with actual address
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
        if (!string.Equals(verifiedAddress, user.Wallet.PublicAddress, StringComparison.OrdinalIgnoreCase))
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

    // Check the JWT is valid
    public async Task<UserAccountViewModel> Authenticate(string token)
    {
        Logger.LogInformation("Authenticating a token {Token}", token);
        var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.AuthenticationToken == token);
        if (user == null) throw new InvalidAuthenticationException(token);
        return Mapper.Map<UserAccountViewModel>(user);
    }

    // Generate JWT for a user
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

    // Remove JWT from database
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

    // Check if the user exists in ownership structure for a work
    public async Task<bool> IsShareholder(string address, string rightId)
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