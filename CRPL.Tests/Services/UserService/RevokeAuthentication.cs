using System.Linq;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class RevokeAuthentication
{
    [Test]
    public async Task Should_Revoke()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            await userService.RevokeAuthentication("TEST_TOKEN");

            context.UserAccounts.FirstOrDefault(x => x.AuthenticationToken == "TEST_TOKEN").Should().BeNull();
        }
    }

    [Test]
    public async Task Should_Throw()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);
            await FluentActions.Invoking(async () => await userService.RevokeAuthentication("")).Should().ThrowAsync<UserNotFoundException>();
        }
    }
}