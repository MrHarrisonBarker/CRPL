using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using CRPL.Web.Services.Submitters;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationSubmitter;

[TestFixture]
public class DisputeSubmitter
{
    [Test]
    public async Task Should_Submit()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new DisputeApplication()
            {
                Id = new Guid("7E6C7A18-5EC8-4C35-8DBE-F8A71A0C2E92"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1),
                Status = ApplicationStatus.Incomplete,
                DisputeType = DisputeType.Usage
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);
        
        var submittedApplication = await dbFactory.Context.DisputeApplications.First().Submit(serviceProviderFactory.ServiceProviderMock.Object);

        submittedApplication.Status.Should().Be(ApplicationStatus.Submitted);
    }
}