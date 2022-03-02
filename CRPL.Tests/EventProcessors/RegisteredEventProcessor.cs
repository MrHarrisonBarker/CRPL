using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Background;
using CRPL.Web.Services.Background.EventProcessors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using NUnit.Framework;

namespace CRPL.Tests.EventProcessors;

[TestFixture]
public class RegisteredEventProcessor
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
        new CopyrightRegistrationApplication
        {
            Id = new Guid("392BC10F-B6CC-42BA-9151-02F12E96776A"),
            Status = ApplicationStatus.Submitted,
            AssociatedWork = Works.First()
        }
    };

    [Test]
    public async Task Should_Update_Work_Status()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO
        {
            To = new List<OwnershipStakeContract>
            {
                new() { Owner = TestConstants.TestAccountAddress, Share = 100 }
            },
            RightId = BigInteger.Parse("1")
        }, new FilterLog
        {
            TransactionHash = Works.First().RegisteredTransactionId
        });

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await dbFactory.Context.RegisteredWorks.FirstOrDefaultAsync(x => x.RightId == "1");
        work.Status.Should().Be(RegisteredWorkStatus.Registered);
    }

    [Test]
    public async Task Should_Update_Application_Status()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO
        {
            To = new List<OwnershipStakeContract>
            {
                new() { Owner = TestConstants.TestAccountAddress, Share = 100 }
            },
            RightId = BigInteger.Parse("6")
        }, new FilterLog
        {
            TransactionHash = "TRANSACTION HASH"
        });

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await dbFactory.Context.RegisteredWorks.Include(x => x.AssociatedApplication).FirstOrDefaultAsync(x => x.RightId == "6");
        work.AssociatedApplication.FirstOrDefault(x => x.ApplicationType == ApplicationType.CopyrightRegistration).Status.Should().Be(ApplicationStatus.Complete);
    }

    [Test]
    public async Task Should_Sign()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO
        {
            To = new List<OwnershipStakeContract>
            {
                new() { Owner = TestConstants.TestAccountAddress, Share = 100 }
            },
            RightId = BigInteger.Parse("1")
        }, new FilterLog
        {
            TransactionHash = Works.First().RegisteredTransactionId
        });

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        serviceProviderFactory.WorksVerificationServiceMock.Verify(x => x.Sign(It.IsAny<RegisteredWork>()), Times.Once);
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var eventLog = new EventLog<RegisteredEventDTO>(new RegisteredEventDTO(), new FilterLog()
        {
            TransactionHash = "BAD HASH"
        });

        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<WorkNotFoundException>();
    }
}