using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetWork
{
    [Test]
    public async Task Should_Get_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var queryService = new QueryServiceFactory().Create(context, null);

            var work = await queryService.GetWork(new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"));

            work.Should().NotBeNull();
            work.Title.Should().BeEquivalentTo("Hello world");
        }
    }
}