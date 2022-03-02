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
public class GetAccount
{
    [Test]
    public async Task Should_Get_Account()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("6CC10777-415D-464B-95E4-C5BE83453C3E"),
                Wallet = new UserWallet { PublicAddress = TestConstants.TestAccountAddress }
            }
        });

        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var status = await userServiceFactory.UserService.GetAccount(new Guid("6CC10777-415D-464B-95E4-C5BE83453C3E"));

        status.UserAccount.Should().NotBeNull();
        status.UserAccount.WalletPublicAddress.Should().Be(TestConstants.TestAccountAddress);
    }

    [Test]
    public async Task Should_Get_Partial_Fields_If_Not_Complete()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("6CC10777-415D-464B-95E4-C5BE83453C3E"),
                Wallet = new UserWallet { PublicAddress = TestConstants.TestAccountAddress }
            }
        });

        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var status = await userServiceFactory.UserService.GetAccount(new Guid("6CC10777-415D-464B-95E4-C5BE83453C3E"));

        status.Should().NotBeNull();
        status.PartialFields.Count().Should().BePositive();
        status.PartialFields.First(x => x.Field == "RegisteredJurisdiction").Type.Should().BeEquivalentTo("string");
    }

    [Test]
    public async Task Should_Throw_Not_Found()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await userServiceFactory.UserService.GetAccount(Guid.Empty)).Should().ThrowAsync<UserNotFoundException>();
    }
}