using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class Authenticate
{
    [Test]
    public async Task Should_Throw()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await userServiceFactory.UserService.Authenticate("")).Should().ThrowAsync<InvalidAuthenticationException>();
    }

    [Test]
    public async Task Should_Authenticate()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet { PublicAddress = "0xaea270413700371a8a28ab8b5ece05201bdf49de" },
                AuthenticationToken = "TEST_TOKEN"
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.Authenticate("TEST_TOKEN")).Should().NotBeNull();
    }
}