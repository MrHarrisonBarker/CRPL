using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.RegistrationService;

[TestFixture]
public class CompleteRegistration
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
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Status = ApplicationStatus.Submitted,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                AssociatedUsers = new List<UserApplication>
                {
                    new() { UserId = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C") }
                },
                AssociatedWork = new RegisteredWork
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now,
                    Title = "Hello world",
                    Status = RegisteredWorkStatus.Verified,
                    WorkType = WorkType.Image
                }
            },
            new CopyrightRegistrationApplication
            {
                Id = new Guid("CB9EE1A2-3842-4B51-AFF6-8BFFC383278A"),
                Status = ApplicationStatus.Submitted,
                Created = DateTime.Now,
                AssociatedWork = new RegisteredWork
                {
                    Id = Guid.NewGuid(),
                    Status = RegisteredWorkStatus.ProcessingVerification,
                    Created = DateTime.Now,
                    Title = "Not verified",
                    WorkType = WorkType.Image
                }
            }
        };

        Users = new List<UserAccount>
        {
            new()
            {
                Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
                Status = UserAccount.AccountStatus.Complete,
                Wallet = new UserWallet { PublicAddress = "0x0000000000000000000000000000000000099991" }
            }
        };
    }

    [Test]
    public async Task Should_Complete()
    {
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_sendTransaction"] = "TEST TRANSACTION";

        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: Users, applications: Applications);
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context, mappings);

        var registeredWork = await registrationServiceFactory.RegistrationService.CompleteRegistration(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        registeredWork.Should().NotBeNull();
        registeredWork.RegisteredTransactionId.Should().BeEquivalentTo("TEST TRANSACTION");
    }

    [Test]
    public async Task Should_Set_Status_To_Sent()
    {
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_sendTransaction"] = "TEST TRANSACTION";

        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: Users, applications: Applications);
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context, mappings);

        var registeredWork = await registrationServiceFactory.RegistrationService.CompleteRegistration(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        registeredWork.Should().NotBeNull();
        registeredWork.Status.Should().Be(RegisteredWorkStatus.SentToChain);
    }

    [Test]
    public async Task Should_Set_Status_Failed_When_Throw()
    {
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_sendTransaction"] = new Exception("TEST EXCEPTION");

        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: Users, applications: Applications);
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context, mappings);

        var applicationId = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30");

        await FluentActions.Invoking(async () => await registrationServiceFactory.RegistrationService.CompleteRegistration(applicationId))
            .Should().ThrowAsync<Exception>();

        var application = await dbFactory.Context.CopyrightRegistrationApplications
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.Id == applicationId);

        application.Status.Should().Be(ApplicationStatus.Failed);
        application.AssociatedWork.Status.Should().Be(RegisteredWorkStatus.Rejected);
    }

    [Test]
    public async Task Should_Not_Set_Failed_If_Wrong_Throw()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: Users, applications: Applications);
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context);

        var applicationId = new Guid("CB9EE1A2-3842-4B51-AFF6-8BFFC383278A");

        await FluentActions.Invoking(async () => await registrationServiceFactory.RegistrationService.CompleteRegistration(applicationId))
            .Should().ThrowAsync<WorkNotVerifiedException>();

        var application = await dbFactory.Context.CopyrightRegistrationApplications
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.Id == applicationId);

        application.Status.Should().NotBe(ApplicationStatus.Failed);
        application.AssociatedWork.Status.Should().NotBe(RegisteredWorkStatus.Rejected);
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await registrationServiceFactory.RegistrationService.CompleteRegistration(Guid.Empty))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_Associated_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = Guid.NewGuid(),
                Title = "Hello world"
            }
        });
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await registrationServiceFactory.RegistrationService.CompleteRegistration(dbFactory.Context.CopyrightRegistrationApplications.First().Id))
            .Should().ThrowAsync<Exception>().WithMessage("There is no work associated with this application!");
    }
}