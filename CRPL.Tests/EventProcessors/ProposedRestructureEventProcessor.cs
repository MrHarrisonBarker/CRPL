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
public class ProposedRestructureEventProcessor
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
            AssociatedWork = Works.First(),
            BindStatus = BindStatus.NoProposal
        }
    };
    
    [Test]
    public async Task Should_Update_Status()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var eventLog = new EventLog<ProposedRestructureEventDTO>(new ProposedRestructureEventDTO()
        {
            RightId = BigInteger.Parse("1")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        var application = await dbFactory.Context.OwnershipRestructureApplications.FirstOrDefaultAsync(x => x.Id == Applications.First().Id);

        application.BindStatus.Should().Be(BindStatus.AwaitingVotes);
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);
        
        var eventLog = new EventLog<ProposedRestructureEventDTO>(new ProposedRestructureEventDTO()
        {
            RightId = BigInteger.MinusOne
        }, new FilterLog());
        
        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }
}