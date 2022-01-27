using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Nethereum.Signer;
using NUnit.Framework;
using NUnit.Framework.Internal;

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

    [Test]
    public async Task Should_Generate_Authentication_Token()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);
            
            var sig = new EthereumMessageSigner().EncodeUTF8AndSign("Signing a unique nonce NONCE", new EthECKey(TestConstants.TestAccountPrivateKey));

            var result = await userService.AuthenticateSignature(new AuthenticateSignatureInputModel()
            {
                Signature = sig,
                WalletAddress = TestConstants.TestAccountAddress
            });

            result.Token.Should().NotBeNullOrEmpty();
            result.Token.Should().NotBeEquivalentTo("TEST_TOKEN");
        }
    }

    [Test]
    public async Task Should_Regenerate_Nonce()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);
            
            var sig = new EthereumMessageSigner().EncodeUTF8AndSign("Signing a unique nonce NONCE", new EthECKey(TestConstants.TestAccountPrivateKey));

            var result = await userService.AuthenticateSignature(new AuthenticateSignatureInputModel()
            {
                Signature = sig,
                WalletAddress = TestConstants.TestAccountAddress
            });

            context.UserAccounts.First(x => x.Id == new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C")).Wallet.Nonce.Should().NotBeNullOrEmpty();
        }
    }
}