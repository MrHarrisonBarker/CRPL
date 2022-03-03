using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class AreUsersReal
{
    [Test]
    public async Task Should_Be_Real()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ADDRESS" }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        userServiceFactory.UserService.AreUsersReal(new List<string> { "ADDRESS" }).Should().BeTrue();
    }

    [Test]
    public async Task Should_Many_Be_Real()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ADDRESS" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ANOTHER_ADDRESS" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ALSO_ANOTHER_ADDRESS" }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        userServiceFactory.UserService.AreUsersReal(new List<string> { "ADDRESS", "ALSO_ANOTHER_ADDRESS", "ANOTHER_ADDRESS" }).Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Ne_Real()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ADDRESS" }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        userServiceFactory.UserService.AreUsersReal(new List<string> { "NON_ADDRESS" }).Should().BeFalse();
    }

    [Test]
    public async Task Should_At_Least_One_Be_Not_Real()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ADDRESS" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ANOTHER_ADDRESS" }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        userServiceFactory.UserService.AreUsersReal(new List<string> { "ADDRESS", "ALSO_ANOTHER_ADDRESS", "ANOTHER_ADDRESS" }).Should().BeFalse();
    }
}