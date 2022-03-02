using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class FetchNonce
{
    [Test]
    public async Task Should_Fetch_New()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet { PublicAddress = TestConstants.TestAccountAddress }
            }
        });

        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var nonce = await userServiceFactory.UserService.FetchNonce(TestConstants.TestAccountAddress);

        nonce.Should().NotBeNull();
        nonce.Length.Should().Be(64);
    }

    [Test]
    public async Task Should_Save_Nonce()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                }
            }
        });

        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await userServiceFactory.UserService.FetchNonce(TestConstants.TestAccountAddress);

        var nonce = dbFactory.Context.UserAccounts.First(x => x.Wallet.PublicAddress == TestConstants.TestAccountAddress).Wallet.Nonce;

        nonce.Should().NotBeNull();
        nonce.Length.Should().Be(64);
    }

    [Test]
    public async Task Should_Save_New_Account()
    {
        using var dbFactory = new TestDbApplicationContextFactory();

        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var nonce = await userServiceFactory.UserService.FetchNonce("random_address");

        nonce.Should().NotBeNull();

        var user = dbFactory.Context.UserAccounts.First(x => x.Wallet.PublicAddress == "random_address");

        user.Should().NotBeNull();
    }
}