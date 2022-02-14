using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetAll
{
    [Test]
    public async Task Should_Get_All()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var queryService = new QueryServiceFactory().Create(context, null);

            var works = await queryService.GetAll(0);
            works.Should().NotBeNull();
            works.Should().NotContainNulls();
            works.Count.Should().Be(context.RegisteredWorks.Count(x => x.Status != RegisteredWorkStatus.Created));
        }
    }
}