using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class IsUniqueEmail
{
    [Test]
    public async Task Should_Be_Unique()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            (await userService.IsUniqueEmail("unique@test.co.uk")).Should().BeTrue();
        }
    }

    [Test]
    public async Task Should_Not_Be_Unique()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            (await userService.IsUniqueEmail("mail@harrisonbarker.co.uk")).Should().BeFalse();
        }
    }
}