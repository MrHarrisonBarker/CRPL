using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.DisputeService;

[TestFixture]
public class RestructureAndResolve
{
    private List<RegisteredWork> Works;
    private List<Application> Applications;
    private List<UserAccount> Users;

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
            new DisputeApplication
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                Status = ApplicationStatus.Submitted,
                ExpectedRecourse = ExpectedRecourse.ChangeOfOwnership,
                AssociatedWork = Works.First()
            },
            new DisputeApplication
            {
                Id = new Guid("16DBABE4-B3C6-4CA6-8FC7-5CD55A25A425"),
                Status = ApplicationStatus.Submitted,
                ExpectedRecourse = ExpectedRecourse.ChangeOfOwnership
            },
            new DisputeApplication
            {
                Id = new Guid("A687FEDC-91B0-447E-A35B-7EAE27803A1A"),
                Status = ApplicationStatus.Submitted,
                ExpectedRecourse = ExpectedRecourse.Payment,
                AssociatedWork = Works.First()
            },
            new DisputeApplication
            {
                Id = new Guid("E75F36C0-8141-412A-8F5F-2CE722D54C6A"),
                Status = ApplicationStatus.Incomplete,
                ExpectedRecourse = ExpectedRecourse.ChangeOfOwnership,
                AssociatedWork = Works.First()
            }
        };

        Users = new List<UserAccount>
        {
            new()
            {
                Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
                Email = "test@user.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Complete",
                LastName = "User",
                PhoneNumber = "99999999999",
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB
                {
                    Year = 2000, Month = 7, Day = 24
                },
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                },
                AuthenticationToken = null,
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
                Applications = new List<UserApplication>
                {
                    new()
                    {
                        Application = Applications.First()
                    }
                }
            }
        };
    }

    [Test]
    public async Task Should_Create_And_Submit_Restructure()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users))
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_call"] =
                "0x0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000100000000000000000000000012890d2cce102216644c59dae5baed380d84830c0000000000000000000000000000000000000000000000000000000000000064";

            var disputeService = new DisputeServiceFactory().Create(context, mappings);

            await disputeService.RestructureAndResolve(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"));

            var restructure = await context.OwnershipRestructureApplications.FirstOrDefaultAsync();

            restructure.Should().NotBeNull();
            restructure.Status.Should().Be(ApplicationStatus.Submitted);
            // Proposal doesn't exist until event processed
            restructure.BindStatus.Should().Be(BindStatus.NoProposal);
            restructure.AssociatedUsers.Count().Should().Be(2);
            restructure.AssociatedWork.Id.Should().Be(new Guid("0FB0C1C0-B3C6-4C1B-88BE-9DCC53D4DAA5"));
        }
    }

    [Test]
    public async Task Should_Throw_When_Not_Submitted()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users))
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await disputeService.RestructureAndResolve(new Guid("E75F36C0-8141-412A-8F5F-2CE722D54C6A")))
                .Should().ThrowAsync<Exception>();
        }
    }

    [Test]
    public async Task Should_Throw_When_Not_ChangeOfOwnership()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users))
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await disputeService.RestructureAndResolve(new Guid("A687FEDC-91B0-447E-A35B-7EAE27803A1A")))
                .Should().ThrowAsync<Exception>();
        }
    }

    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await disputeService.RestructureAndResolve(Guid.Empty))
                .Should().ThrowAsync<DisputeNotFoundException>();
        }
    }

    [Test]
    public async Task Should_Throw_When_No_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users))
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await disputeService.RestructureAndResolve(new Guid("16DBABE4-B3C6-4CA6-8FC7-5CD55A25A425")))
                .Should().ThrowAsync<Exception>();
        }
    }
}