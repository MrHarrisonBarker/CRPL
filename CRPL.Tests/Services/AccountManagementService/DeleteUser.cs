using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nethereum.Web3;
using NUnit.Framework;

namespace CRPL.Tests.Services.AccountManagementService;

[TestFixture]
public class DeleteUser
{
    private List<RegisteredWork> Works;
    private List<Application> Applications;
    private List<UserAccount> UserAccounts;

    [SetUp]
    public async Task SetUp()
    {
        Works = new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("0FB0C1C0-B3C6-4C1B-88BE-9DCC53D4DAA5"),
                Title = "Hello world",
                Status = RegisteredWorkStatus.Registered,
                Created = DateTime.Now,
                Registered = DateTime.Now,
                RightId = "1"
            }
        };

        Applications = new List<Application>
        {
            new DeleteAccountApplication
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                Status = ApplicationStatus.Submitted,
                AssociatedWork = Works.First(),
                AccountId = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C")
            }
        };

        UserAccounts = new List<UserAccount>
        {
            new()
            {
                Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
                Status = UserAccount.AccountStatus.Complete,
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                },
                UserWorks = new List<UserWork>
                {
                    new()
                    {
                        RegisteredWork = Works.First()
                    }
                }
            },
            new()
            {
                Id = new Guid("F61BB4E5-E1C7-4F3E-A39A-93ABAFFE1AC9"),
                Status = UserAccount.AccountStatus.Complete,
                Wallet = new UserWallet
                {
                    PublicAddress = "0xaea270413700371a8a28ab8b5ece05201bdf49de"
                },
                UserWorks = new List<UserWork>
                {
                    new()
                    {
                        RegisteredWork = Works.First()
                    }
                }
            }
        };
    }
    
    [Test]
    public async Task Should_Propose_When_Multi_Ownership()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications, UserAccounts))
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_call"] =
                "0x0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000200000000000000000000000012890d2cce102216644c59dae5baed380d84830c0000000000000000000000000000000000000000000000000000000000000037000000000000000000000000aea270413700371a8a28ab8b5ece05201bdf49de000000000000000000000000000000000000000000000000000000000000002d";

            var (accountManagementService, formsServiceMock) = new AccountManagementServiceFactory().Create(context, mappings);

            formsServiceMock.Setup(x => x.Update<OwnershipRestructureViewModel>(It.IsAny<ApplicationInputModel>())).ReturnsAsync(new OwnershipRestructureViewModel()
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A")
            });
            
            var application = await context.DeleteAccountApplications.FirstOrDefaultAsync(x => x.Id == new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"));

            await accountManagementService.DeleteUser(application);
            
            formsServiceMock.Verify(x => x.Update<OwnershipRestructureViewModel>(It.IsAny<ApplicationInputModel>()));
            formsServiceMock.Verify(x => x.Submit<OwnershipRestructureApplication, OwnershipRestructureViewModel>(application.Id));
        }
    }
}