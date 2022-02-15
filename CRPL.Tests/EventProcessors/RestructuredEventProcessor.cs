using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Applications;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Background;
using CRPL.Web.Services.Background.EventProcessors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using NUnit.Framework;

namespace CRPL.Tests.EventProcessors;

[TestFixture]
public class RestructuredEventProcessor
{
    [Test]
    public async Task Should_Assign_Shareholders()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = TestConstants.TestAccountAddress, Share = 45 },
                    new() { Owner = "test_2", Share = 55 }
                }
            },
            RightId = BigInteger.Parse("6")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.RightId == "6");

        work.Should().NotBeNull();
        work.UserWorks[0].UserAccount.Wallet.PublicAddress.Should().BeEquivalentTo(TestConstants.TestAccountAddress);
        work.UserWorks[1].UserAccount.Wallet.PublicAddress.Should().BeEquivalentTo("test_2");
    }

    [Test]
    public async Task Should_Set_Status()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = TestConstants.TestAccountAddress, Share = 45 },
                    new() { Owner = "test_2", Share = 55 }
                }
            },
            RightId = BigInteger.Parse("6")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));
        
        var application = await context.OwnershipRestructureApplications.FirstOrDefaultAsync(x => x.Id == new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"));

        application.BindStatus.Should().Be(BindStatus.Bound);
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            RightId = BigInteger.MinusOne
        }, new FilterLog());
        
        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<WorkNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_User()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = "INVALID ADDRESS", Share = 100 },
                }
            },
            RightId = BigInteger.Parse("6")
        }, new FilterLog());
        
        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<UserNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = TestConstants.TestAccountAddress, Share = 100 }
                }
            },
            RightId = BigInteger.Parse("1")
        }, new FilterLog());
        
        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }
}