using System;
using System.Collections.Generic;
using System.Data.Common;
using CRPL.Data.Account;
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
                    PublicAddress = "test_0"
                }
            },
        };
        
        context.UserAccounts.AddRange(userAccounts);
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