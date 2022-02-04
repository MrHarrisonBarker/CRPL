using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class GetApplication
{
    [Test]
    public async Task Should_Map_To_Registration()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new FormsServiceFactory().Create(context);

            var application = await userService.GetApplication(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));
        }
    }
}