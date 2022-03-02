using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class GetMyApplications
{
    private List<Application> Applications;
    private List<UserAccount> Users;

    [SetUp]
    public async Task SetUp()
    {
        Applications = new List<Application>
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            },
            new OwnershipRestructureApplication
            {
                Id = new Guid("A71C7C89-A35B-45F8-8DEE-D3CC084CC0F9"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            },
            new DeleteAccountApplication
            {
                Id = new Guid("CC577F00-114B-498F-BFBA-95DC9D420DEB"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            },
            new DisputeApplication
            {
                Id = new Guid("C3B6995D-5E7C-4BA8-96B9-091A65C9FFEB"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            },
            new WalletTransferApplication
            {
                Id = new Guid("A42501EB-025E-4FFF-9BF3-BD4B1CF19DFF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        };

        Users = new List<UserAccount>
        {
            new()
            {
                Id = new Guid("8086DDF9-F841-4D74-85BB-E1A80D71FE79"),
                Wallet = new UserWallet { PublicAddress = "ADDRESS" },
                Applications = Applications.Select(x => new UserApplication()
                {
                    Application = x
                }).ToList()
            }
        };
    }

    [Test]
    public async Task Should_Get_Users_Applications()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: Applications, userAccounts: Users);
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var applications = await formsServiceFactory.FormsService.GetMyApplications(new Guid("8086DDF9-F841-4D74-85BB-E1A80D71FE79"));

        applications.Count.Should().BePositive();
        applications.Should().BeOfType<List<ApplicationViewModel>>();
    }
}