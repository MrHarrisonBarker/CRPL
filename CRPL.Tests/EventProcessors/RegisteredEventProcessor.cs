using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Account;
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
public class RegisteredEventProcessor
{
    
    
    [Test]
    public async Task Should_Update_Work_Status()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO
        {
            To = new List<OwnershipStakeContract>
            {
                new() {Owner = TestConstants.TestAccountAddress, Share = 100}
            },
            RightId = BigInteger.Parse("6")
        }, new FilterLog()
        {
            TransactionHash = "TRANSACTION HASH"
        });

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await context.RegisteredWorks.FirstOrDefaultAsync(x => x.RightId == "6");
        work.Status.Should().Be(RegisteredWorkStatus.Registered);
    }

    [Test]
    public async Task Should_Update_Application_Status()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO
        {
            To = new List<OwnershipStakeContract>
            {
                new() {Owner = TestConstants.TestAccountAddress, Share = 100}
            },
            RightId = BigInteger.Parse("6")
        }, new FilterLog
        {
            TransactionHash = "TRANSACTION HASH"
        });

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await context.RegisteredWorks.FirstOrDefaultAsync(x => x.RightId == "6");
        work.AssociatedApplication.FirstOrDefault(x => x.ApplicationType == ApplicationType.CopyrightRegistration).Status.Should().Be(ApplicationStatus.Complete);
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO(), new FilterLog()
        {
            TransactionHash = "BAD HASH"
        });
        
        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<WorkNotFoundException>();
    }
}