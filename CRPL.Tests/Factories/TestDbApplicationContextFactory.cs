using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
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

    public ApplicationContext CreateContext(List<RegisteredWork> registeredWorks = null, List<Application> applications = null, List<UserAccount> userAccounts = null)
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
                if (userAccounts != null) context.UserAccounts.AddRange(userAccounts);
                if (registeredWorks != null) context.RegisteredWorks.AddRange(registeredWorks);
                if (applications != null) context.Applications.AddRange(applications);
                context.SaveChanges();
            }
        }

        return new ApplicationContext(CreateOptions());
    }

    private void seed(ApplicationContext context)
    {
        List<UserAccount> userAccounts = new()
        {
            new UserAccount
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
            new UserAccount
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
            new UserAccount
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
            new UserAccount
            {
                Id = new Guid("F61BB4E5-E1C7-4F3E-A39A-93ABAFFE1AC9"),
                Email = "harrison@thebarkers.me.uk",
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
                    PublicAddress = "0xaea270413700371a8a28ab8b5ece05201bdf49de"
                },
                AuthenticationToken = "TEST_TOKEN"
            },
            new UserAccount
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
                RightId = "1",
                Title = "Hello world",
                Registered = DateTime.Now.AddDays(-1),
                Status = RegisteredWorkStatus.Registered
            },
            new()
            {
                Hash = new byte[] { 0, 0 },
                Id = new Guid("C96560FD-6528-4921-9650-761AE96EF0DA"),
                RightId = "2",
                Title = "test title",
                Registered = DateTime.Now.AddDays(-2),
                Status = RegisteredWorkStatus.Registered
            },
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("E2199DB5-DC40-4690-B812-4E52A4D74A06"),
                RightId = "3",
                Title = "another title",
                Registered = DateTime.Now.AddDays(-3),
                Status = RegisteredWorkStatus.Registered
            },
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("85B77C6F-7D0C-4FCE-9691-4613C1F8BFDE"),
                RightId = "4",
                Title = "Created",
                Status = RegisteredWorkStatus.Created
            },
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("816FE428-4350-4124-B855-72A429C925A6"),
                RightId = "5",
                Title = "Verified",
                Status = RegisteredWorkStatus.Verified
            },
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("9EE1AEF2-47BA-4A13-8AFE-693CF3D7E3DD"),
                RightId = "6",
                Title = "Assigned",
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now.AddDays(-1),
                RegisteredTransactionId = "TRANSACTION HASH"
            },
        };

        List<Application> applications = new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                AssociatedUsers = new List<UserApplication>()
                {
                    new() { UserId = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C") }
                },
            },
            new OwnershipRestructureApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"),
                ApplicationType = ApplicationType.OwnershipRestructure,
                CurrentStructure = "ADDRESS!50;ANOTHER_ADDRESS!50",
                ProposedStructure = "ADDRESS!90;ANOTHER_ADDRESS!10",
            },
            new CopyrightRegistrationApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("CDBEE1A0-D266-43AB-BB0A-16E3CD07451E"),
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                Status = ApplicationStatus.Submitted,
                AssociatedWork = registeredWorks.First(x => x.Id == new Guid("816FE428-4350-4124-B855-72A429C925A6")),
            },
            new OwnershipRestructureApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"),
                ApplicationType = ApplicationType.OwnershipRestructure,
                CurrentStructure = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                ProposedStructure = "0x0000000000000000000000000000000000099991!90;0x0000000000000000000000000000000000099992!10",
                AssociatedWork = registeredWorks.Last(),
                Status = ApplicationStatus.Submitted
            },
            new CopyrightRegistrationApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("E4D79015-9228-498A-9B16-3F76CB14104D"),
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                AssociatedWork = new()
                {
                    Title = "ProcessingVerification",
                    Created = DateTime.Now,
                    Status = RegisteredWorkStatus.ProcessingVerification
                },
            },
            new CopyrightRegistrationApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("807DADCF-9629-410D-8A36-4C366B5D53F5"),
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                Status = ApplicationStatus.Complete
            },
            new CopyrightRegistrationApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("57F0DC07-889D-446B-8E4D-D45DA4B4DCC4"),
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                Status = ApplicationStatus.Submitted
            },
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