using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Contracts.Structs;
using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;
using CRPL.Tests.Factories;
using CRPL.Web.Services;
using CRPL.Web.Services.Updaters;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationUpdater;

[TestFixture]
public class CopyrightRegistrationUpdater
{
    [Test]
    public async Task Should_Update()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>()));
        serviceProviderFactory.UserServiceMock.Setup(x => x.AssignToApplication(It.IsAny<string>(), It.IsAny<Guid>()));

        var updatedApplication = (CopyrightRegistrationApplication)await dbFactory.Context.Applications.First().UpdateApplication(new CopyrightRegistrationInputModel
        {
            Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
            Legal = "LEGAL",
            Protections = new Protections()
            {
                Authorship = true,
                CommercialAdaptation = true,
                CommercialDistribution = true,
                CommercialPerformance = true,
                CommercialReproduction = true,
                NonCommercialAdaptation = true,
                NonCommercialDistribution = true,
                NonCommercialPerformance = true,
                NonCommercialReproduction = true,
                ReviewOrCrit = true
            },
            Title = "HELLO WORLD",
            WorkUri = "http://www.harrisonbarker.co.uk",
            WorkHash = new byte[] { 0, 0, 0 },
            WorkType = WorkType.Image,
            YearsExpire = 100
        }, serviceProviderFactory.ServiceProviderMock.Object);

        updatedApplication.Legal.Should().BeEquivalentTo("LEGAL");
        updatedApplication.Protections.Authorship.Should().BeTrue();
        updatedApplication.Title.Should().BeEquivalentTo("HELLO WORLD");
        updatedApplication.WorkUri.Should().BeEquivalentTo("http://www.harrisonbarker.co.uk");
        updatedApplication.WorkHash.Should().BeEquivalentTo(new byte[] { 0, 0, 0 });
        updatedApplication.WorkType.Should().Be(WorkType.Image);
        updatedApplication.YearsExpire.Should().Be(100);

        updatedApplication.Modified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    [Test]
    public async Task Should_Update_And_Encode_Ownership_Stakes()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);
        
        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);
        
        var updatedApplication = await dbFactory.Context.CopyrightRegistrationApplications.First().Update(new CopyrightRegistrationInputModel
        {
            OwnershipStakes = new List<OwnershipStake>
            {
                new() {Owner = "ADDRESS_1", Share = 24},
                new() {Owner = "ADDRESS_2", Share = 76}
            }
        }, serviceProviderFactory.ServiceProviderMock.Object);

        updatedApplication.OwnershipStakes.Should().BeEquivalentTo("ADDRESS_1!24;ADDRESS_2!76;");
    }

    [Test]
    public async Task Should_Assign_Owners()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);
        
        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);
        
        await dbFactory.Context.CopyrightRegistrationApplications.First().Update(new CopyrightRegistrationInputModel
        {
            OwnershipStakes = new List<OwnershipStake>
            {
                new() {Owner = "ADDRESS_1", Share = 24},
                new() {Owner = "ADDRESS_2", Share = 76}
            }
        }, serviceProviderFactory.ServiceProviderMock.Object);
        
        serviceProviderFactory.UserServiceMock.Verify(x => x.AssignToApplication("address_1", new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF")), Times.Once);
        serviceProviderFactory.UserServiceMock.Verify(x => x.AssignToApplication("address_2", new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF")), Times.Once);
    }

    [Test]
    public async Task Should_Throw_If_No_Shareholder()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(false);
        serviceProviderFactory.UserServiceMock.Setup(x => x.AssignToApplication(It.IsAny<string>(), It.IsAny<Guid>()));

        await FluentActions.Invoking(async () => await dbFactory.Context.Applications.First().UpdateApplication(new CopyrightRegistrationInputModel
        {
            OwnershipStakes = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } }
        }, serviceProviderFactory.ServiceProviderMock.Object)).Should().ThrowAsync<Exception>().WithMessage("Not all the users could be found");
    }
}