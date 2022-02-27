using System;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationSubmitter;

[TestFixture]
public class CopyrightRegistrationSubmitter
{
    [Test]
    public async Task Should_Submit()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (applicationContext, serviceProvider, copyrightServiceMock, accountManagementServiceMock, registrationServiceMock) = new ApplicationSubmitterFactory().Create(context);
            
            var application = new CopyrightRegistrationApplication()
            {
                Id = new Guid("7E6C7A18-5EC8-4C35-8DBE-F8A71A0C2E92"),
                Status = ApplicationStatus.Incomplete
            };

            var submitted = await application.Submit(serviceProvider);
        
            registrationServiceMock.Verify(x => x.StartRegistration(application));
            submitted.Status.Should().Be(ApplicationStatus.Submitted);
        }
    }
}