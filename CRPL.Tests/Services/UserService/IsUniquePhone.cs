using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class IsUniquePhone
{
    [Test]
    public async Task Should_Be_Unique()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.IsUniquePhoneNumber("+4400000000000")).Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Be_Unique()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet { PublicAddress = "" },
                PhoneNumber = "07852276048"
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.IsUniquePhoneNumber("07852276048")).Should().BeFalse();
    }
}