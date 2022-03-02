using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetUsersWorks
{
    private List<RegisteredWork> Works;
    private List<UserAccount> Users;

    [SetUp]
    public async Task SetUp()
    {
        Works = new List<RegisteredWork>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Hello world",
                Created = DateTime.Now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Another Hello world",
                Created = DateTime.Now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Another another Hello world",
                Created = DateTime.Now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Another another another Hello world",
                Created = DateTime.Now
            }
        };

        Users = new List<UserAccount>
        {
            new()
            {
                Id = new Guid("8086DDF9-F841-4D74-85BB-E1A80D71FE79"),
                Wallet = new UserWallet { PublicAddress = "ADDRESS" },
                UserWorks = Works.Select(x => new UserWork()
                {
                    RegisteredWork = x
                }).ToList()
            }
        };
    }

    [Test]
    public async Task Should_Get_Users_Works()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, userAccounts: Users);
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var works = await queryServiceFactory.QueryService.GetUsersWorks(new Guid("8086DDF9-F841-4D74-85BB-E1A80D71FE79"));

        works.Should().NotBeNull();
        works.Should().NotContainNulls();
        works.Count.Should().Be(dbFactory.Context.RegisteredWorks.Count());
        works.Count.Should().BePositive();
    }
}