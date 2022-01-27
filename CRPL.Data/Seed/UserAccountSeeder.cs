using CRPL.Data.Account;

namespace CRPL.Data.Seed;

public class UserAccountSeeder
{
    private readonly ApplicationContext Context;

    public UserAccountSeeder(ApplicationContext context)
    {
        Context = context;
    }
    
    public void Seed()
    {
        List<UserAccount> userAccounts = new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"),
                Email = null,
                Status = UserAccount.AccountStatus.Incomplete,
                FirstName = "Incomplete",
                LastName = "User",
                PhoneNumber = null,
                RegisteredJurisdiction = null,
                DateOfBirth = null,
                Wallet = new UserWallet()
                {
                    PublicAddress = "1"
                }
            },
            new()
            {
                Id = new Guid("73C4FF17-1EF8-483C-BCDB-9A6191888F04"),
                Email = "mail@harrisonbarker.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Complete",
                LastName = "User",
                DialCode = "+44",
                PhoneNumber = "07852276048",
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB()
                {
                    Year = 2000, Month = 7, Day = 24
                },
                Wallet = new UserWallet()
                {
                    PublicAddress = "2"
                }
            },
            new()
            {
                Id = new Guid("8E9C6FB8-A8D7-459F-A39C-B06E68FE4E03"),
                Email = "",
                Status = UserAccount.AccountStatus.Created,
                FirstName = "",
                LastName = "",
                PhoneNumber = "",
                RegisteredJurisdiction = "",
                DateOfBirth = new UserAccount.DOB(),
                Wallet = new UserWallet()
                {
                    PublicAddress = "0"
                }
            },
        };

        Context.UserAccounts.AddRange(userAccounts);
        Context.SaveChanges();
    }
}