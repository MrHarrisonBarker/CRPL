using System.Security.Cryptography;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Data.Account.ViewModels;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

// all methods effecting user accounts and user wallets
public class UserService : IUserService
{
    private readonly ILogger<UserService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;

    public UserService(ILogger<UserService> logger, ApplicationContext context, IMapper mapper)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
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
        var ignoredProperties = new List<string> { "Wallet", "RegisteredWorks"};
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
        var ignoredProperties = new List<string> { "Email", "PhoneNumber", "Wallet", "RegisteredWorks"};
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
    public async Task<byte[]> FetchNonce(string walletAddress)
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
                    PublicAddress = walletAddress
                }
            };
        }

        user.Wallet.Nonce = generateNonce();

        await Context.SaveChangesAsync();
        
        return user.Wallet.Nonce;
    }

    private byte[] generateNonce()
    {
        Logger.LogInformation("Generating new nonce");
        
        var arr = new byte[32];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(arr);
        return arr;
    }

    public Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel)
    {
        throw new NotImplementedException();
    }
}