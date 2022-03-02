using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class GetApplication
{
    [Test]
    public async Task Should_Map_To_Registration()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                AssociatedUsers = new List<UserApplication>
                {
                    new() { UserId = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C") }
                },
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var application = await formsServiceFactory.FormsService.GetApplication(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        application.Should().NotBeNull();
        application.Should().BeOfType<CopyrightRegistrationViewModel>();
    }

    [Test]
    public async Task Should_Map_To_Ownership()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new OwnershipRestructureApplication
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"),
                ApplicationType = ApplicationType.OwnershipRestructure,
                CurrentStructure = "ADDRESS!50;ANOTHER_ADDRESS!50",
                ProposedStructure = "ADDRESS!90;ANOTHER_ADDRESS!10",
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var application = await formsServiceFactory.FormsService.GetApplication(new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"));

        application.Should().NotBeNull();
        application.Should().BeOfType<OwnershipRestructureViewModel>();
    }

    [Test]
    public async Task Should_Map_To_Dispute()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new DisputeApplication
            {
                Id = new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"),
                DisputeType = DisputeType.Ownership,
                Reason = "REASON"
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var application = await formsServiceFactory.FormsService.GetApplication(new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"));

        application.Should().NotBeNull();
        application.Should().BeOfType<DisputeViewModel>();
    }

    [Test]
    public async Task Should_Map_To_Delete()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new DeleteAccountApplication
            {
                Id = new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"),
                AccountId = Guid.NewGuid()
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var application = await formsServiceFactory.FormsService.GetApplication(new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"));

        application.Should().NotBeNull();
        application.Should().BeOfType<DeleteAccountViewModel>();
    }

    [Test]
    public async Task Should_Map_To_Wallet_Transfer()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new WalletTransferApplication
            {
                Id = new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"),
                WalletAddress = "WALLET_ADDRESS"
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var application = await formsServiceFactory.FormsService.GetApplication(new Guid("83EB5EDF-43BA-4F34-B14F-219F85B0FF5F"));

        application.Should().NotBeNull();
        application.Should().BeOfType<WalletTransferViewModel>();
    }
}