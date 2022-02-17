using System;
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
public class RestructuredEventProcessor
{
    private List<RegisteredWork> Works;
    private List<Application> Applications;
    private List<UserAccount> Users;

    [SetUp]
    public async Task Setup()
    {
        Works = new()
        {
            new()
            {
                Id = new Guid("C714A94E-BE61-4D7B-A4CE-28F0667FAEAD"),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                RightId = "1",
                RegisteredTransactionId = "TRANSACTION HASH"
            },
            new()
            {
                Id = new Guid("A6F20E18-9EBA-48E7-A6C9-6EBA0D591FD1"),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                RightId = "2",
                RegisteredTransactionId = "TRANSACTION HASH"
            }
        };

        Applications = new()
        {
            new OwnershipRestructureApplication
            {
                Id = new Guid("392BC10F-B6CC-42BA-9151-02F12E96776A"),
                Status = ApplicationStatus.Submitted,
                AssociatedWork = Works.First()
            }
        };

        Users = new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet {PublicAddress = TestConstants.TestAccountAddress}
            },
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet {PublicAddress = "test_2"}
            }
        };
    }

    [Test]
    public async Task Should_Assign_Shareholders()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory()
            .Create(new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users));

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
            RightId = BigInteger.Parse("1")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.RightId == "1");

        work.Should().NotBeNull();
        work.UserWorks[0].UserAccount.Wallet.PublicAddress.Should().BeEquivalentTo(TestConstants.TestAccountAddress);
        work.UserWorks[1].UserAccount.Wallet.PublicAddress.Should().BeEquivalentTo("test_2");
    }

    [Test]
    public async Task Should_Set_Status()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory()
            .Create(new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users));

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = TestConstants.TestAccountAddress, Share = 45 },
                    new() { Owner = "test_2", Share = 55 }
                }
            },
            RightId = BigInteger.Parse("1")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory()));

        var application = await context.OwnershipRestructureApplications.FirstOrDefaultAsync(x => x.Id == Applications.First().Id);

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
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory()
            .Create(new TestDbApplicationContextFactory().CreateContext(Works, Applications));

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = "INVALID ADDRESS", Share = 100 },
                }
            },
            RightId = BigInteger.Parse("1")
        }, new FilterLog());

        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<UserNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        var (context, serviceProvider) = await new ServiceProviderWithContextFactory()
            .Create(new TestDbApplicationContextFactory().CreateContext(Works, Applications, Users));

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            Proposal = new RestructureProposal
            {
                NewStructure = new List<OwnershipStakeContract>
                {
                    new() { Owner = TestConstants.TestAccountAddress, Share = 100 }
                }
            },
            RightId = BigInteger.Parse("2")
        }, new FilterLog());

        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProvider, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }
}