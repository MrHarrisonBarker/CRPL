using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class IsUniqueEmail
{
    [Test]
    public async Task Should_Be_Unique()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.IsUniqueEmail("unique@test.co.uk")).Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Be_Unique()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet {PublicAddress = ""},
                Email = "mail@harrisonbarker.co.uk"
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.IsUniqueEmail("mail@harrisonbarker.co.uk")).Should().BeFalse();
    }
}