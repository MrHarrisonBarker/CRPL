using System;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
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
        var application = new OwnershipRestructureApplication
        {
            Id = new Guid("7E6C7A18-5EC8-4C35-8DBE-F8A71A0C2E92"),
            Status = ApplicationStatus.Incomplete
        };

        var registrationServiceMock = new Mock<IRegistrationService>();
        var copyrightServiceMock = new Mock<ICopyrightService>();

        copyrightServiceMock.Setup(x => x.ProposeRestructure(application)).ReturnsAsync(application);

        var submitted = await application.Submit(registrationServiceMock.Object, copyrightServiceMock.Object);

        copyrightServiceMock.Verify(x => x.ProposeRestructure(application));
        submitted.Status.Should().Be(ApplicationStatus.Submitted);
    }
}