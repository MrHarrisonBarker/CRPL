using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Services.Submitters;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationSubmitter;

[TestFixture]
public class CopyrightRegistrationSubmitter
{
    [Test]
    public async Task Should_Submit_And_Start_Registration()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("7E6C7A18-5EC8-4C35-8DBE-F8A71A0C2E92"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1),
                Status = ApplicationStatus.Incomplete
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var submittedApplication = await dbFactory.Context.CopyrightRegistrationApplications.First().Submit(serviceProviderFactory.ServiceProviderMock.Object);

        serviceProviderFactory.RegistrationServiceMock.Verify(x => x.StartRegistration(It.IsAny<CopyrightRegistrationApplication>()),Times.Once);
        submittedApplication.Status.Should().Be(ApplicationStatus.Submitted);
    }
}