using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using CRPL.Tests.Factories.Synchronisers;
using CRPL.Tests.Mocks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nethereum.ABI.FunctionEncoding;
using NUnit.Framework;

namespace CRPL.Tests.Synchronisers.OwnershipSynchroniser;

[TestFixture]
public class SynchroniseOne
{
    private List<RegisteredWork> Works;
    private List<UserAccount> Users;

    [SetUp]
    public async Task SetUp()
    {
        Works = new List<RegisteredWork>()
        {
            new()
            {
                Id = new Guid("C714A94E-BE61-4D7B-A4CE-28F0667FAEAD"),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.SentToChain,
                RightId = "1",
                RegisteredTransactionId = "TRANSACTION HASH",
                UserWorks = new List<UserWork>
                {
                    new()
                    {
                        UserId = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C")
                    }
                }
            }
        };
        
        Users = new List<UserAccount>
        {
            new()
            {
                Id = new Guid("F61BB4E5-E1C7-4F3E-A39A-93ABAFFE1AC9"),
                Wallet = new UserWallet
                {
                    PublicAddress = "0xaea270413700371a8a28ab8b5ece05201bdf49de"
                }
            },
            new()
            {
                Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
                Email = "test@user.co.uk",
                Status = UserAccount.AccountStatus.Complete,
                FirstName = "Complete",
                LastName = "User",
                PhoneNumber = "99999999999",
                RegisteredJurisdiction = "GBR",
                DateOfBirth = new UserAccount.DOB
                {
                    Year = 2000, Month = 7, Day = 24
                },
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                },
                AuthenticationToken = null
            }
        };
    }

    [Test]
    public async Task Should_Be_The_Same()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, userAccounts: Users);

        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_call"] =
            "0x0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000100000000000000000000000012890d2cce102216644c59dae5baed380d84830c0000000000000000000000000000000000000000000000000000000000000064";

        var ownershipSynchroniserFactory = new OwnershipSynchroniserFactory(dbFactory.Context, mappings);

        await ownershipSynchroniserFactory.OwnershipSynchroniser.SynchroniseOne(new Guid("C714A94E-BE61-4D7B-A4CE-28F0667FAEAD"));
    }

    [Test]
    public async Task Should_Be_Different()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, userAccounts: Users);
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_call"] =
            "0x00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000001000000000000000000000000aea270413700371a8a28ab8b5ece05201bdf49de0000000000000000000000000000000000000000000000000000000000000064";

        var ownershipSynchroniserFactory = new OwnershipSynchroniserFactory(dbFactory.Context, mappings);

        await ownershipSynchroniserFactory.OwnershipSynchroniser.SynchroniseOne(Works.First().Id);

        var work = dbFactory.Context.RegisteredWorks
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .FirstOrDefault(x => x.Id == Works.First().Id);

        work.UserWorks.Count.Should().Be(1);
        work.UserWorks.First().UserAccount.Wallet.PublicAddress.Should().BeEquivalentTo("0xaea270413700371a8a28ab8b5ece05201bdf49de");
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var ownershipSynchroniserFactory = new OwnershipSynchroniserFactory(dbFactory.Context);

        FluentActions.Invoking(async () => await ownershipSynchroniserFactory.OwnershipSynchroniser.SynchroniseOne(Guid.Empty));
    }

    [Test]
    public async Task Should_Catch_Expired_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, userAccounts: Users);
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_call"] = new SmartContractRevertException("EXPIRED", "");
        
        var ownershipSynchroniserFactory = new OwnershipSynchroniserFactory(dbFactory.Context, mappings);

        await ownershipSynchroniserFactory.OwnershipSynchroniser.SynchroniseOne(Works.First().Id);
        
        ownershipSynchroniserFactory.ExpiryQueueMock.Verify(x => x.QueueExpire(Works.First().Id), Times.Once);
    }
}