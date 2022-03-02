using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class RevokeAuthentication
{
    [Test]
    public async Task Should_Revoke()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = new Guid("45C89178-DB68-476C-8B23-269FA6675821"),
                AuthenticationToken = "TEST_TOKEN",
                Wallet = new UserWallet() { PublicAddress = "TEST ADDRESS" }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await userServiceFactory.UserService.RevokeAuthentication("TEST_TOKEN");

        dbFactory.Context.UserAccounts.FirstOrDefault(x => x.AuthenticationToken == "TEST_TOKEN").Should().BeNull();
    }

    [Test]
    public async Task Should_Throw()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);
        
        await FluentActions.Invoking(async () => await userServiceFactory.UserService.RevokeAuthentication("")).Should().ThrowAsync<UserNotFoundException>();
    }
}