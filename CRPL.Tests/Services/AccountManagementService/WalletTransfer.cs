using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.Services.AccountManagementService;

[TestFixture]
public class WalletTransfer
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

        Applications = new List<Application>()
        {
            new WalletTransferApplication()
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                WalletAddress = "TEST_ADDRESS"
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
                },
                Applications = Applications.Select(x => new UserApplication()
                {
                    Application = x
                }).ToList()
            }
        };
    }

    [Test]
    public async Task Should_Propose()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications, UserAccounts))
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_call"] =
                "0x0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000200000000000000000000000012890d2cce102216644c59dae5baed380d84830c0000000000000000000000000000000000000000000000000000000000000037000000000000000000000000aea270413700371a8a28ab8b5ece05201bdf49de000000000000000000000000000000000000000000000000000000000000002d";
            
            var (accountManagementService, formsServiceMock) = new AccountManagementServiceFactory().Create(context, mappings);

            formsServiceMock.Setup(x => x.Update<OwnershipRestructureViewModel>(It.IsAny<ApplicationInputModel>())).Returns<ApplicationInputModel>((inputModel) =>
                Task.FromResult(new OwnershipRestructureViewModel()
                {
                    Id = new Guid("186A2071-525D-4347-AF09-129745623C15")
                }));
            
            var application = await context.WalletTransferApplications.Include(x => x.AssociatedUsers).FirstOrDefaultAsync(x => x.Id == new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"));

            await accountManagementService.WalletTransfer(application);
            formsServiceMock.Verify(x => x.Update<OwnershipRestructureViewModel>(It.IsAny<ApplicationInputModel>()), Times.Once);
            formsServiceMock.Verify(x => x.Submit<OwnershipRestructureApplication, OwnershipRestructureViewModel>(new Guid("186A2071-525D-4347-AF09-129745623C15")), Times.Once);
        }
    }
}