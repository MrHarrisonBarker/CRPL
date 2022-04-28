using System;
using System.Collections.Generic;
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
        using var dbFactory = new TestDbApplicationContextFactory();
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await userServiceFactory.UserService.AuthenticateSignature(new AuthenticateSignatureInputModel())).Should().ThrowAsync<UserNotFoundException>();
    }

    [Test]
    public async Task Should_Be_Invalid()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet { PublicAddress = "test_2" }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var signature = "0x425bf720bd2a076fe9a523c1e77ddc6a99e659fd0553c7a2bed7aa57599c602e397191cef61b85ec7641dc7d087536bcfe6ed6b9eb89e8dbc0160d1e5d87c4dc1b";

        await FluentActions.Invoking(async () => await userServiceFactory.UserService.AuthenticateSignature(new AuthenticateSignatureInputModel()
        {
            WalletAddress = "test_2",
            Signature = signature
        })).Should().ThrowAsync<InvalidSignatureException>().WithMessage($"Invalid signature! {signature} did not match the address test_2");
    }

    [Test]
    public async Task Should_Generate_Authentication_Token()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var sig = new EthereumMessageSigner().EncodeUTF8AndSign("Signing a unique nonce NONCE", new EthECKey(TestConstants.TestAccountPrivateKey));

        var result = await userServiceFactory.UserService.AuthenticateSignature(new AuthenticateSignatureInputModel
        {
            Signature = sig,
            WalletAddress = TestConstants.TestAccountAddress
        });

        result.Token.Should().NotBeNullOrEmpty();
        result.Token.Should().NotBeEquivalentTo("TEST_TOKEN");
    }

    [Test]
    public async Task Should_Authenticate_For_30_Days()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var sig = new EthereumMessageSigner().EncodeUTF8AndSign("Signing a unique nonce NONCE", new EthECKey(TestConstants.TestAccountPrivateKey));

        var result = await userServiceFactory.UserService.AuthenticateSignature(new AuthenticateSignatureInputModel()
        {
            Signature = sig,
            WalletAddress = TestConstants.TestAccountAddress
        });

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        token.ValidTo.Should().BeAfter(DateTime.Now.AddDays(29));
    }

    [Test]
    public async Task Should_Return_User_If_Authenticated()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var sig = new EthereumMessageSigner().EncodeUTF8AndSign("Signing a unique nonce NONCE", new EthECKey(TestConstants.TestAccountPrivateKey));

        var result = await userServiceFactory.UserService.AuthenticateSignature(new AuthenticateSignatureInputModel()
        {
            Signature = sig,
            WalletAddress = TestConstants.TestAccountAddress
        });

        result.Account.Should().NotBeNull();
        result.Account.WalletPublicAddress.Should().BeEquivalentTo(TestConstants.TestAccountAddress);
    }

    [Test]
    public async Task Should_Regenerate_Nonce()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
                Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
                Wallet = new UserWallet
                {
                    PublicAddress = TestConstants.TestAccountAddress,
                    Nonce = "NONCE"
                }
            }
        });
        var userServiceFactory = new UserServiceFactory(dbFactory.Context);

        var sig = new EthereumMessageSigner().EncodeUTF8AndSign("Signing a unique nonce NONCE", new EthECKey(TestConstants.TestAccountPrivateKey));

        var result = await userServiceFactory.UserService.AuthenticateSignature(new AuthenticateSignatureInputModel()
        {
            Signature = sig,
            WalletAddress = TestConstants.TestAccountAddress
        });

        dbFactory.Context.UserAccounts.First(x => x.Id == new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C")).Wallet.Nonce.Should().NotBeNullOrEmpty();
    }
}