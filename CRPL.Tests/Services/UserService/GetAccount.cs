using System;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class GetAccount
{
    [Test]
    public async Task Should_Get_Account()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            var status = await userService.GetAccount(TestConstants.TestAccountIds[UserAccount.AccountStatus.Complete]);

            status.UserAccount.Should().NotBeNull();
            status.UserAccount.Id.Should().Be(TestConstants.TestAccountIds[UserAccount.AccountStatus.Complete]);
        }
    }

    [Test]
    public async Task Should_Get_Wallet_Address_With_Account()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            var status = await userService.GetAccount(TestConstants.TestAccountIds[UserAccount.AccountStatus.Complete]);

            status.UserAccount.Should().NotBeNull();
            status.UserAccount.WalletPublicAddress.Should().Be("test_2");
        } 
    }

    [Test]
    public async Task Should_Get_Partial_Fields_If_Not_Complete()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            var status = await userService.GetAccount(TestConstants.TestAccountIds[UserAccount.AccountStatus.Incomplete]);

            status.Should().NotBeNull();
            status.PartialFields.Count().Should().BePositive();
            status.PartialFields.First(x => x.Field == "RegisteredJurisdiction").Type.Should().BeEquivalentTo("string");
        }
    }

    [Test]
    public async Task Should_Throw_Not_Found()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await userService.GetAccount(Guid.Empty)).Should().ThrowAsync<UserNotFoundException>();
        }
    }
}