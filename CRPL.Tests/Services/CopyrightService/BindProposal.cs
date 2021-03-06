using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Proposal;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.CopyrightService;

[TestFixture]
public class BindProposal
{
    private List<RegisteredWork> Works;

    private List<Application> Applications;

    [SetUp]
    public async Task SetUp()
    {
        Works = new()
        {
            new RegisteredWork
            {
                Id = new Guid("2D098895-6EA9-4DB6-B84A-16E4F66EAE2C"),
                Title = "Hello world",
                Status = RegisteredWorkStatus.Registered,
                Created = DateTime.Now,
                RightId = "1"
            }
        };

        Applications = new()
        {
            new OwnershipRestructureApplication
            {
                Id = new Guid("5E1643E8-9DA7-42A3-BB2C-6A9F26F0FA46"),
                AssociatedWork = Works.First()
            }
        };
    }

    [Test]
    public async Task Should_Send_Transaction_With_Application()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        await copyrightServiceFactory.CopyrightService.BindProposal(new BindProposalInput { ApplicationId = Applications.First().Id, Accepted = true });
    }

    [Test]
    public async Task Should_Send_Transaction_With_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        await copyrightServiceFactory.CopyrightService.BindProposal(new BindProposalWorkInput { WorkId = Works.First().Id, Accepted = true });
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await copyrightServiceFactory.CopyrightService.BindProposal(new BindProposalInput { ApplicationId = Guid.Empty }))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await copyrightServiceFactory.CopyrightService.BindProposal(new BindProposalWorkInput { WorkId = Guid.Empty }))
            .Should().ThrowAsync<WorkNotFoundException>();
    }
}