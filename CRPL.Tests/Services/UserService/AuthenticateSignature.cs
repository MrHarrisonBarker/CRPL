using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Nethereum.Signer;
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

            var signature = "0x425bf720bd2a076fe9a523c1e77ddc6a99e659fd0553c7a2bed7aa57599c602e397191cef61b85ec7641dc7d087536bcfe6ed6b9eb89e8dbc0160d1e5d87c4dc1b";
            var address = TestConstants.TestAccountWallets[UserAccount.AccountStatus.Complete];

            await FluentActions.Invoking(async () => await userService.AuthenticateSignature(new AuthenticateSignatureInputModel()
            {
                WalletAddress = address,
                Signature = signature
            })).Should().ThrowAsync<InvalidSignatureException>().WithMessage($"Invalid signature! {signature} did not match the address {address}");
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
    public async Task Should_Authenticate_For_30_Days()
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

            var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
            token.ValidTo.Should().BeAfter(DateTime.Now.AddDays(29).AddHours(23));
        }
    }

    [Test]
    public async Task Should_Return_User_If_Authenticated()
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
            
            result.Account.Should().NotBeNull();
            result.Account.WalletPublicAddress.Should().BeEquivalentTo(TestConstants.TestAccountAddress);
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