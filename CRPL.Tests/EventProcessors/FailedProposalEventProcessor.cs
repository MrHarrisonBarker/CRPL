using System;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
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
public class FailedProposalEventProcessor
{
    [Test]
    public async Task Should_Set_Status()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();

        var eventLog = new EventLog<FailedProposalEventDTO>(new FailedProposalEventDTO()
        {
            RightId = BigInteger.Parse("6")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var application = await context.OwnershipRestructureApplications.FirstOrDefaultAsync(x => x.Id == new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"));

        application.BindStatus.Should().Be(BindStatus.Rejected);
        application.Status.Should().Be(ApplicationStatus.Failed);
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory().Create();
        
        var eventLog = new EventLog<FailedProposalEventDTO>(new FailedProposalEventDTO()
        {
            RightId = BigInteger.MinusOne
        }, new FilterLog());
        
        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }
}