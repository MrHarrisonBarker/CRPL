using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
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
public class FailedProposalEventProcessor
{
    private static readonly List<RegisteredWork> Works = new()
    {
        new()
        {
            Id = new Guid("C714A94E-BE61-4D7B-A4CE-28F0667FAEAD"),
            Title = "Hello world",
            Created = DateTime.Now,
            Status = RegisteredWorkStatus.SentToChain,
            RightId = "1",
            RegisteredTransactionId = "TRANSACTION HASH"
        }
    };

    private static readonly List<Application> Applications = new()
    {
        new OwnershipRestructureApplication
        {
            Id = new Guid("392BC10F-B6CC-42BA-9151-02F12E96776A"),
            Status = ApplicationStatus.Submitted,
            AssociatedWork = Works.First()
        }
    };
    
    [Test]
    public async Task Should_Set_Status()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory()
            .Create(new TestDbApplicationContextFactory().CreateContext(Works, Applications));

        var eventLog = new EventLog<FailedProposalEventDTO>(new FailedProposalEventDTO()
        {
            RightId = BigInteger.Parse("1")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var application = await context.OwnershipRestructureApplications.FirstOrDefaultAsync(x => x.Id == Applications.First().Id);

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