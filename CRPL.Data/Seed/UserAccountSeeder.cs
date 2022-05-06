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
        List<UserAccount> userAccounts = new List<UserAccount>
        {
            // new()
            // {
            //     Id = Guid.NewGuid(),
            //     Email = null,
            //     Status = UserAccount.AccountStatus.Complete,
            //     FirstName = "Harrison",
            //     LastName = "Barker",
            //     PhoneNumber = "07852276048",
            //     RegisteredJurisdiction = "GBR",
            //     DateOfBirth = new UserAccount.DOB
            //     {
            //         Day = 24,
            //         Month = 7,
            //         Year = 2000
            //     },
            //     DialCode = "+44",
            //     Wallet = new UserWallet
            //     {
            //         PublicAddress = "0x3Aaf677eA4e72eEbB92d2D5c3A92307EE789E24c"
            //     }
            // },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "Roy.Batty@test.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Roy",
                LastName = "Batty",
                PhoneNumber = null,
                DialCode = null,
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB
                {
                    Day = 1,
                    Month = 1,
                    Year = 2000
                },
                Wallet = new UserWallet
                {
                    PublicAddress = "0xaEa270413700371A8A28Ab8B5eCe05201bdf49de"
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "Rick.Deckard@test.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Rick",
                LastName = "Deckard",
                PhoneNumber = null,
                DialCode = null,
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB
                {
                    Day = 1,
                    Month = 1,
                    Year = 2000
                },
                Wallet = new UserWallet
                {
                    PublicAddress = "0x270C23E93A85e79aEE96eb061e3DadCBC58CAdB0"
                }
            },
        };

        Context.UserAccounts.AddRange(userAccounts);
        Context.SaveChanges();
    }
}