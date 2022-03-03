using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class IsShareholder
{
    [Test]
    public async Task Should_Be_Share_Holder()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = new Guid("C3A0B252-7ED5-49E5-9CE9-CE6A38DBA9A4"),
                Wallet = new() { PublicAddress = "ADDRESS" }
            }
        }, registeredWorks: new List<RegisteredWork>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                RightId = "1",
                Title = "Hello world",
                Created = DateTime.Now,
                WorkType = WorkType.Image,
                UserWorks = new List<UserWork>()
                {
                    new() { UserId = new Guid("C3A0B252-7ED5-49E5-9CE9-CE6A38DBA9A4") }
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.IsShareholder("ADDRESS", "1")).Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Be_Share_Holder()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new() { PublicAddress = "ADDRESS" }
            }
        }, registeredWorks: new List<RegisteredWork>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                RightId = "1",
                Title = "Hello world",
                Created = DateTime.Now,
                WorkType = WorkType.Image,
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        (await userServiceFactory.UserService.IsShareholder("ADDRESS", "1")).Should().BeFalse();
    }

    [Test]
    public async Task Should_Throw_If_No_User()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await userServiceFactory.UserService.IsShareholder("ADDRESS", "1")).Should().ThrowAsync<UserNotFoundException>();
    }
}