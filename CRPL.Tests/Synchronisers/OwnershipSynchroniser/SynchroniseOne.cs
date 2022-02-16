using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Tests.Factories;
using CRPL.Tests.Factories.Synchronisers;
using CRPL.Tests.Mocks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Synchronisers.OwnershipSynchroniser;

[TestFixture]
public class SynchroniseOne
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
            RegisteredTransactionId = "TRANSACTION HASH",
            UserWorks = new List<UserWork>()
            {
                new() {UserAccount = TestDBUtils.UserAccounts.Last()}
            }
        }
    };

    [Test]
    public async Task Should_Be_The_Same()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, new List<Application>()))
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_call"] = "0x0000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000000000000000000100000000000000000000000012890d2cce102216644c59dae5baed380d84830c0000000000000000000000000000000000000000000000000000000000000064";
            
            var ownershipSynchroniser = new OwnershipSynchroniserFactory().Create(context, mappings);

            await ownershipSynchroniser.SynchroniseOne(new Guid("C714A94E-BE61-4D7B-A4CE-28F0667FAEAD"));
        }
    }

    [Test]
    public async Task Should_Be_Different()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, new List<Application>()))
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_call"] = "0x00000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000001000000000000000000000000aea270413700371a8a28ab8b5ece05201bdf49de0000000000000000000000000000000000000000000000000000000000000064";
            
            var ownershipSynchroniser = new OwnershipSynchroniserFactory().Create(context, mappings);

            await ownershipSynchroniser.SynchroniseOne(Works.First().Id);

            var work = context.RegisteredWorks
                .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
                .FirstOrDefault(x => x.Id == Works.First().Id);

            work.UserWorks.Count.Should().Be(1);
            work.UserWorks.First().UserAccount.Wallet.PublicAddress.Should().BeEquivalentTo("0xaea270413700371a8a28ab8b5ece05201bdf49de");
        }
    }
}