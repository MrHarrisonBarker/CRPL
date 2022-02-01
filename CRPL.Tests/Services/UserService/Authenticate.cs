using System.Threading.Tasks;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class Authenticate
{
    [Test]
    public async Task Should_Throw()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await userService.Authenticate("")).Should().ThrowAsync<InvalidAuthenticationException>();
        }
    }

    [Test]
    public async Task Should_Authenticate()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            (await userService.Authenticate("TEST_TOKEN")).Should().NotBeNull();
        }
    }
}