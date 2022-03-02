using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
public class OwnershipRestructureUpdater
{
    [Test]
    public async Task Should_Update()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new OwnershipRestructureApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);
        serviceProviderFactory.UserServiceMock.Setup(x => x.AssignToApplication(It.IsAny<string>(), It.IsAny<Guid>()));

        var currentStructure = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } };
        var proposedStructure = new List<OwnershipStake>
        {
            new() { Owner = "test_0", Share = 300 },
            new() { Owner = "test_1", Share = 10 }
        };

        var updatedApplication = await dbFactory.Context.OwnershipRestructureApplications.First().Update(new OwnershipRestructureInputModel
        {
            Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
            CurrentStructure = currentStructure,
            ProposedStructure = proposedStructure,
            WorkId = new Guid("762F1CD0-DF85-4651-AEEA-15312783527F"),
            RestructureReason = RestructureReason.Application
        }, serviceProviderFactory.ServiceProviderMock.Object);

        updatedApplication.CurrentStructure.Should().BeEquivalentTo(currentStructure.Encode());
        updatedApplication.ProposedStructure.Should().BeEquivalentTo(proposedStructure.Encode());
        
        serviceProviderFactory.CopyrightServiceMock.Verify(x => x.AttachWorkToApplicationAndCheckValid(It.IsAny<Guid>(), It.IsAny<Application>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task Should_Assign_Users()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new OwnershipRestructureApplication
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);
        
        await dbFactory.Context.OwnershipRestructureApplications.First().Update(new OwnershipRestructureInputModel
        {
            CurrentStructure = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } },
            ProposedStructure = new List<OwnershipStake>
            {
                new() { Owner = "test_0", Share = 300 },
                new() { Owner = "test_1", Share = 10 }
            }
        }, serviceProviderFactory.ServiceProviderMock.Object);
        
        serviceProviderFactory.UserServiceMock.Verify(x => x.AssignToApplication(It.IsAny<string>(), It.IsAny<Guid>()), Times.Exactly(2));
    }
    
    [Test]
    public async Task Should_Throw_If_No_Shareholder()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new OwnershipRestructureApplication()
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.UserServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(false);
        serviceProviderFactory.UserServiceMock.Setup(x => x.AssignToApplication(It.IsAny<string>(), It.IsAny<Guid>()));

        await FluentActions.Invoking(async () => await dbFactory.Context.OwnershipRestructureApplications.First().UpdateApplication(new OwnershipRestructureInputModel
        {
            CurrentStructure = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } },
            ProposedStructure = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } }
        }, serviceProviderFactory.ServiceProviderMock.Object)).Should().ThrowAsync<Exception>().WithMessage("Not all the users could be found");
    }
}