using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class AuthenticateSignature
{
    [Test]
    public async Task Should_Throw_Not_Found()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await userService.AuthenticateSignature(new AuthenticateSignatureInputModel())).Should().ThrowAsync<UserNotFoundException>();
        }
    }

    [Test]
    public async Task Should_Be_Invalid()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);
            
            await FluentActions.Invoking(async () => await userService.AuthenticateSignature(new AuthenticateSignatureInputModel()
            {
                WalletAddress = TestConstants.TestAccountWallets[UserAccount.AccountStatus.Complete],
                Signature = "0x425bf720bd2a076fe9a523c1e77ddc6a99e659fd0553c7a2bed7aa57599c602e397191cef61b85ec7641dc7d087536bcfe6ed6b9eb89e8dbc0160d1e5d87c4dc1b"
            })).Should().ThrowAsync<InvalidSignature>();
        }
    }
}