using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
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
            },
            new()
            {
                Id = new Guid("09D6E87D-6073-4976-A6D8-492F035F24B5"),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                RightId = "3",
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
            },
            new OwnershipRestructureApplication
            {
                Id = new Guid("4981A1FE-A9B9-4D73-A11A-5630E832D08D"),
                Status = ApplicationStatus.Submitted,
                AssociatedWork = Works.FirstOrDefault(x => x.RightId == "3"),
                Origin = new DisputeApplication()
                {
                    Id = new Guid("F3828B1A-90FC-4088-87BD-F89CA61DF677"),
                    Status = ApplicationStatus.Submitted
                }
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
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications, Users);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

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

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        var work = await dbFactory.Context.RegisteredWorks
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
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications, Users);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

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

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        var application = await dbFactory.Context.OwnershipRestructureApplications.FirstOrDefaultAsync(x => x.Id == Applications.First().Id);

        application.BindStatus.Should().Be(BindStatus.Bound);
        application.Status.Should().Be(ApplicationStatus.Complete);
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var eventLog = new EventLog<RestructuredEventDTO>(new RestructuredEventDTO()
        {
            RightId = BigInteger.MinusOne
        }, new FilterLog());

        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<WorkNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_User()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

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

        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<UserNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications, Users);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

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

        await FluentActions.Invoking(async () => await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory())))
            .Should().ThrowAsync<ApplicationNotFoundException>();
    }

    [Test]
    public async Task Should_Set_Origin_To_Complete()
    {
        using var dbFactory = new TestDbApplicationContextFactory(Works, Applications, Users);
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

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
            RightId = BigInteger.Parse("3")
        }, new FilterLog());

        await eventLog.ProcessEvent(serviceProviderFactory.ServiceProviderMock.Object, new Logger<EventProcessingService>(new LoggerFactory()));

        var application = await dbFactory.Context.OwnershipRestructureApplications.Include(x => x.Origin).FirstOrDefaultAsync(x => x.Id == new Guid("4981A1FE-A9B9-4D73-A11A-5630E832D08D"));

        application.Origin.Status.Should().Be(ApplicationStatus.Complete);
    }
}