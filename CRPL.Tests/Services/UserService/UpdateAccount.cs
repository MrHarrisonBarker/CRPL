using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class UpdateAccount
{
    [Test]
    public async Task Should_Update_UserAccount()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"),
                Email = null,
                Status = UserAccount.AccountStatus.Incomplete,
                FirstName = "Incomplete",
                LastName = "User",
                PhoneNumber = null,
                RegisteredJurisdiction = null,
                DateOfBirth = null,
                Wallet = new UserWallet()
                {
                    PublicAddress = "test_1"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var status = await userServiceFactory.UserService.UpdateAccount(new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"), new AccountInputModel
        {
            RegisteredJurisdiction = "USA",
            DateOfBirth = new UserAccount.DOB
            {
                Day = 16,
                Month = 1,
                Year = 1974
            }
        });

        status.UserAccount.Status.Should().Be(UserAccount.AccountStatus.Incomplete);
        status.UserAccount.RegisteredJurisdiction.Should().BeEquivalentTo("USA");
        status.UserAccount.DateOfBirth.Day.Should().Be(16);
        status.UserAccount.DateOfBirth.Month.Should().Be(1);
        status.UserAccount.DateOfBirth.Year.Should().Be(1974);

        var userAccount = dbFactory.Context.UserAccounts.First(x => x.Id == new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"));

        userAccount.RegisteredJurisdiction.Should().BeEquivalentTo("USA");
        userAccount.DateOfBirth.Day.Should().Be(16);
        userAccount.DateOfBirth.Month.Should().Be(1);
        userAccount.DateOfBirth.Year.Should().Be(1974);
    }

    [Test]
    public async Task Should_Update_Status_When_Complete()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"),
                Email = null,
                Status = UserAccount.AccountStatus.Incomplete,
                FirstName = "Incomplete",
                LastName = "User",
                PhoneNumber = null,
                RegisteredJurisdiction = null,
                DateOfBirth = null,
                Wallet = new UserWallet()
                {
                    PublicAddress = "test_1"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var status = await userServiceFactory.UserService.UpdateAccount(new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"), new AccountInputModel
        {
            Email = "test@test.co.uk",
            DialCode = "0",
            PhoneNumber = "1",
            RegisteredJurisdiction = "USA",
            DateOfBirth = new UserAccount.DOB
            {
                Day = 16,
                Month = 1,
                Year = 1974
            },
        });

        status.PartialFields.Count().Should().Be(0);
        status.UserAccount.Status.Should().Be(UserAccount.AccountStatus.Complete);

        var userAccount = dbFactory.Context.UserAccounts.First(x => x.Id == new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"));

        userAccount.Status.Should().Be(UserAccount.AccountStatus.Complete);
    }

    [Test]
    public async Task Should_Return_Partials()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"),
                Email = null,
                Status = UserAccount.AccountStatus.Incomplete,
                FirstName = "Incomplete",
                LastName = "User",
                PhoneNumber = null,
                RegisteredJurisdiction = null,
                DateOfBirth = null,
                Wallet = new UserWallet()
                {
                    PublicAddress = "test_1"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var status = await userServiceFactory.UserService.UpdateAccount(new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C"), new AccountInputModel());

        status.PartialFields.Count().Should().BePositive();
        status.PartialFields.First(x => x.Field == "RegisteredJurisdiction").Type.Should().BeEquivalentTo("string");
    }

    [Test]
    public async Task Should_Throw_Not_Found()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await userServiceFactory.UserService.UpdateAccount(Guid.Empty, new AccountInputModel())).Should().ThrowAsync<UserNotFoundException>();
    }
}