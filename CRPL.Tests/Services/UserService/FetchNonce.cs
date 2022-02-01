using System.Linq;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class FetchNonce
{
    [Test]
    public async Task Should_Fetch_New()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            var nonce = await userService.FetchNonce("test_2");

            nonce.Should().NotBeNull();
            nonce.Length.Should().Be(64);
        }
    }
    
    [Test]
    public async Task Should_Save_Nonce()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            await userService.FetchNonce("test_2");

            var nonce = context.UserAccounts.First(x => x.Wallet.PublicAddress == "test_2").Wallet.Nonce;
            
            nonce.Should().NotBeNull();
            nonce.Length.Should().Be(64);
        }
    }

    [Test]
    public async Task Should_Save_New_Account()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);
            
            var nonce = await userService.FetchNonce("random_address");
            
            nonce.Should().NotBeNull();

            var user = context.UserAccounts.First(x => x.Wallet.PublicAddress == "random_address");

            user.Should().NotBeNull();
        }
    }
}