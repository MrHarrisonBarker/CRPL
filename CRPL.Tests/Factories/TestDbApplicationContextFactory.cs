using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Tests.Factories;

public class TestDbApplicationContextFactory : IDisposable
{
    private DbConnection? Connection;

    private DbContextOptions<ApplicationContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationContext>()
            .EnableSensitiveDataLogging()
            .UseSqlite(Connection).Options;
    }

    public ApplicationContext CreateContext()
    {
        if (Connection == null)
        {
            Connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");
            Connection.Open();

            var options = CreateOptions();
            using (var context = new ApplicationContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                seed(context);
            }
        }

        return new ApplicationContext(CreateOptions());
    }


    private void seed(ApplicationContext context)
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
                    PublicAddress = "test_1"
                }
            },
            new()
            {
                Id = new Guid("73C4FF17-1EF8-483C-BCDB-9A6191888F04"),
                Email = "mail@harrisonbarker.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Complete",
                LastName = "User",
                PhoneNumber = "+4407852276048",
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB()
                {
                    Year = 2000, Month = 7, Day = 24
                },
                Wallet = new UserWallet()
                {
                    PublicAddress = "test_2"
                },
                AuthenticationToken = "TEST_TOKEN"
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
                    PublicAddress = "test_0"
                }
            },
            new()
            {
                Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
                Email = "test@user.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Complete",
                LastName = "User",
                PhoneNumber = "99999999999",
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB()
                {
                    Year = 2000, Month = 7, Day = 24
                },
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                },
                AuthenticationToken = null
            }
        };

        List<RegisteredWork> registeredWorks = new List<RegisteredWork>()
        {
            new()
            {
                Hash = new byte[] { 0 },
                Id = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"),
                RightId = "1"
            },
            new()
            {
                Hash = new byte[] { 0, 0 },
                Id = new Guid("C96560FD-6528-4921-9650-761AE96EF0DA"),
                RightId = "2"
            },
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("E2199DB5-DC40-4690-B812-4E52A4D74A06"),
                RightId = "3"
            }
        };

        List<Application> applications = new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "ADDRESS!50;ANOTHER_ADDRESS!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = "HASH",
                WorkUri = "URI"
            }
        };

        context.UserAccounts.AddRange(userAccounts);
        context.RegisteredWorks.AddRange(registeredWorks);
        context.Applications.AddRange(applications);
        context.SaveChanges();
    }

    public void Dispose()
    {
        if (Connection != null)
        {
            Connection.Dispose();
            Connection = null;
        }
    }
}