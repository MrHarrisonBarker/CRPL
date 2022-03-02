using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Tests.Factories;
using CRPL.Web.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationUpdater;

[TestFixture]
public class DisputeUpdater
{
    [Test]
    public async Task Should_Update()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new DisputeApplication()
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var updatedApplication = (DisputeApplication)await dbFactory.Context.Applications.First().UpdateApplication(new DisputeInputModel
        {
            Infractions = 1,
            Reason = "REASON",
            Spotted = DateTime.Today,
            ContactAddress = "test@test.co.uk",
            DisputeType = DisputeType.Ownership,
            ExpectedRecourse = ExpectedRecourse.Payment,
            ExpectedRecourseData = "0.123456",
            LinkToInfraction = "test.co.uk",
            AccuserId = new Guid("61C66E21-3640-45EE-A814-41F1698537DD"),
            DisputedWorkId = new Guid("474B3329-B295-45E2-B963-7F38D567970D")
        }, serviceProviderFactory.ServiceProviderMock.Object);

        updatedApplication.Infractions.Should().Be(1);
        updatedApplication.Reason.Should().BeEquivalentTo("REASON");
        updatedApplication.Spotted.Should().Be(DateTime.Today);
        updatedApplication.ContactAddress.Should().BeEquivalentTo("test@test.co.uk");
        updatedApplication.ExpectedRecourse.Should().Be(ExpectedRecourse.Payment);
        updatedApplication.ExpectedRecourseData.Should().BeEquivalentTo("0.123456");
        updatedApplication.LinkToInfraction.Should().BeEquivalentTo("test.co.uk");
        
        updatedApplication.Modified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        
        serviceProviderFactory.CopyrightServiceMock.Verify(x => x.AttachWorkToApplicationAndCheckValid(new Guid("474B3329-B295-45E2-B963-7F38D567970D"), It.IsAny<Application>()));
        serviceProviderFactory.UserServiceMock.Verify(x => x.AssignToApplication(new Guid("61C66E21-3640-45EE-A814-41F1698537DD"), It.IsAny<Guid>()));
    }
}