using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Tests.Factories;
using CRPL.Web.Services.Submitters;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationSubmitter;

[TestFixture]
public class OwnershipRestructureSubmitter
{
    [Test]
    public async Task Should_Submit()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new OwnershipRestructureApplication()
            {
                Id = new Guid("7E6C7A18-5EC8-4C35-8DBE-F8A71A0C2E92"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1),
                Status = ApplicationStatus.Incomplete
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.CopyrightServiceMock.Setup(x => x.ProposeRestructure(dbFactory.Context.OwnershipRestructureApplications.First())).ReturnsAsync(dbFactory.Context.OwnershipRestructureApplications.First());

        var submittedApplication = await dbFactory.Context.OwnershipRestructureApplications.First().Submit(serviceProviderFactory.ServiceProviderMock.Object);
        
        serviceProviderFactory.CopyrightServiceMock.Verify(x => x.ProposeRestructure(dbFactory.Context.OwnershipRestructureApplications.First()));
        submittedApplication.Status.Should().Be(ApplicationStatus.Submitted);
    }
}