using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetUsersWorks
{
    [Test]
    public async Task Should_Get_Users_Works()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var queryService = new QueryServiceFactory().Create(context, null);

            var works = await queryService.GetUsersWorks(new Guid("73C4FF17-1EF8-483C-BCDB-9A6191888F04"));

            works.Should().NotBeNull();
            works.Should().NotContainNulls();
        }
    }
}