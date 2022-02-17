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
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "07852276048",
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB
                {
                    Day = 24,
                    Month = 7,
                    Year = 2000
                },
                DialCode = "+44",
                Wallet = new UserWallet
                {
                    PublicAddress = "0x3aaf677ea4e72eebb92d2d5c3a92307ee789e24c"
                }
            }
        };

        Context.UserAccounts.AddRange(userAccounts);
        Context.SaveChanges();
    }
}