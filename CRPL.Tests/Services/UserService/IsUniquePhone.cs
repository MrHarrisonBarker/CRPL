using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class IsUniquePhone
{
    [Test]
    public async Task Should_Be_Unique()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            (await userService.IsUniquePhoneNumber("+4400000000000")).Should().BeTrue();
        }
    }

    [Test]
    public async Task Should_Not_Be_Unique()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            (await userService.IsUniquePhoneNumber("+4407852276048")).Should().BeFalse();
        }
    }
}