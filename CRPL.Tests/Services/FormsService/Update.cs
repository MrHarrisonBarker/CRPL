using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;
using CRPL.Tests.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class UpdateTest
{
    [Test]
    public async Task Should_Update()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Title = "NEW TITLE"
            });

            updatedApplication.Title.Should().Be("NEW TITLE");
            updatedApplication.Legal.Should().NotBeNull();
            updatedApplication.OwnershipStakes.Count.Should().BePositive();
        }
    }

    [Test]
    public async Task Should_Update_Ownership_Structure()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var currentStructure = new List<OwnershipStake>
            {
                new() { Owner = "test_0", Share = 100 }
            };

            var proposedStructure = new List<OwnershipStake>
            {
                new() { Owner = "test_0", Share = 300 },
                new() { Owner = "test_1", Share = 10 }
            };

            var updatedApplication = await formsService.Update<OwnershipRestructureViewModel>(new OwnershipRestructureInputModel()
            {
                CurrentStructure = currentStructure,
                ProposedStructure = proposedStructure
            });

            updatedApplication.Should().NotBeNull();
            updatedApplication.CurrentStructure.Should().BeEquivalentTo(currentStructure);
            updatedApplication.ProposedStructure.Should().BeEquivalentTo(proposedStructure);
        }
    }

    [Test]
    public async Task Should_Save_Updates()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Title = "NEW TITLE"
            });

            var savedApplication = (CopyrightRegistrationApplication)(await context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30")));

            savedApplication.Should().NotBeNull();
            savedApplication.Title.Should().Be("NEW TITLE");
        }
    }

    [Test]
    public async Task Should_Update_Stakes()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var ownership = new List<OwnershipStake>
            {
                new() { Owner = "test_0", Share = 300 },
                new() { Owner = "test_1", Share = 10 }
            };

            var updatedApplication = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                OwnershipStakes = ownership
            });

            updatedApplication.Should().NotBeNull();
            updatedApplication.OwnershipStakes.Should().BeEquivalentTo(ownership);
        }
    }

    [Test]
    public async Task Should_Create_New_If_Not_Found()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                OwnershipStakes = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } },
                Title = "TEST APPLICATION"
            });

            updatedApplication.Should().NotBeNull();
            updatedApplication.Title.Should().BeEquivalentTo("TEST APPLICATION");
            updatedApplication.Id.Should().NotBeEmpty();
        }
    }

    [Test]
    public async Task Should_Create_New_And_Save()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                OwnershipStakes = new List<OwnershipStake> { new() { Owner = "test_0", Share = 100 } },
                Title = "TEST APPLICATION"
            });

            updatedApplication.Should().NotBeNull();

            var application = await context.CopyrightRegistrationApplications.FirstOrDefaultAsync(x => x.Title == "TEST APPLICATION");

            application.Should().NotBeNull();
            application.Title.Should().BeEquivalentTo("TEST APPLICATION");
            application.Id.Should().NotBeEmpty();
        }
    }

    [Test]
    public async Task Should_Add_All_Associated_Users()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                OwnershipStakes = new List<OwnershipStake> { new() { Owner = TestConstants.TestAccountAddress, Share = 100 } },
                Title = "TEST APPLICATION"
            });

            updatedApplication.AssociatedUsers.Should().NotBeNull();
            updatedApplication.AssociatedUsers.FirstOrDefault(x => x.Id == new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C")).Should().NotBeNull();
        }
    }

    [Test]
    public async Task Should_Not_Find_Shareholder()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
            {
                OwnershipStakes = new List<OwnershipStake> { new() { Owner = "NON EXISTENT ADDRESS", Share = 100 } },
                Title = "TEST APPLICATION"
            })).Should().ThrowAsync<Exception>();
        }
    }
}