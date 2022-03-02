using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;
using CRPL.Tests.Factories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class UpdateTest
{
    [Test]
    public async Task Should_Save_Updates()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        await formsServiceFactory.FormsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel
        {
            Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
            Title = "NEW TITLE"
        });

        var savedApplication = (CopyrightRegistrationApplication)dbFactory.Context.Applications.First();

        savedApplication.Should().NotBeNull();
        savedApplication.Title.Should().Be("NEW TITLE");
    }

    [Test]
    public async Task Should_Create_New_If_Not_Found()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        formsServiceFactory.ServiceProviderWithContextFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);

        var updatedApplication = await formsServiceFactory.FormsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel
        {
            OwnershipStakes = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } },
            Title = "TEST APPLICATION"
        });

        updatedApplication.Should().NotBeNull();
        updatedApplication.Title.Should().BeEquivalentTo("TEST APPLICATION");
        updatedApplication.Id.Should().NotBeEmpty();
    }

    [Test]
    public async Task Should_Create_New_And_Save()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        formsServiceFactory.ServiceProviderWithContextFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);
        
        var updatedApplication = await formsServiceFactory.FormsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel
        {
            OwnershipStakes = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } },
            Title = "TEST APPLICATION"
        });

        updatedApplication.Should().NotBeNull();

        var application = dbFactory.Context.CopyrightRegistrationApplications.First();

        application.Should().NotBeNull();
        application.Title.Should().BeEquivalentTo("TEST APPLICATION");
        application.Id.Should().NotBeEmpty();
    }
}