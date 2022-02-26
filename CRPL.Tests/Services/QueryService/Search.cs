using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class Search
{
    [Test]
    public async Task Should_Search_Keyword()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var works = await queryService.Search(new StructuredQuery()
            {
                Keyword = "title"
            }, 0);

            works.Should().NotBeNull();
        }
    }

    [Test]
    public async Task Should_Search_Sort_By()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var works = await queryService.Search(new StructuredQuery()
            {
                SortBy = Sortable.Created
            }, 0);

            works.Should().NotBeNull();
            var lastCreated = works.First().Created;
            works.ForEach(work =>
            {
                work.Created.Should().NotBeBefore(lastCreated);
                lastCreated = work.Created;
            });
        }
    }

    [Test]
    public async Task Should_Only_Take_One()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var works = await queryService.Search(new StructuredQuery(), 0, 1);

            works.Should().NotBeNull();
            works.Count.Should().Be(1);
        }
    }

    [Test]
    public async Task Should_Be_After()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var works = await queryService.Search(new StructuredQuery()
            {
                WorkFilters = new Dictionary<WorkFilter, string>()
                {
                    { WorkFilter.RegisteredAfter, DateTime.Now.ToString(CultureInfo.InvariantCulture) }
                }
            }, 0);

            works.Should().NotBeNull();
            works.Count.Should().Be(0);
        }
    }

    [Test] public async Task Should_Be_Before()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var works = await queryService.Search(new StructuredQuery()
            {
                WorkFilters = new Dictionary<WorkFilter, string>()
                {
                    { WorkFilter.RegisteredBefore, DateTime.Now.AddDays(-2).ToString(CultureInfo.InvariantCulture) }
                }
            }, 0);

            works.Should().NotBeNull();
            works.Count.Should().Be(1);
        }
    }

    [Test]
    public async Task Should_Be_Type()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>()
                     {
                         new()
                         {
                             Id = new Guid("E3DB3187-391C-4EB6-9751-48B556BF04AE"),
                             Created = DateTime.Now,
                             Registered = DateTime.Now,
                             Status = RegisteredWorkStatus.Registered,
                             WorkType = WorkType.Image,
                             Title = "Hello world"
                         }
                     }))
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var works = await queryService.Search(new StructuredQuery()
            {
                WorkFilters = new Dictionary<WorkFilter, string>()
                {
                    { WorkFilter.WorkType, "Image" }
                }
            }, 0);

            works.Should().NotBeNull();
            works.Count.Should().Be(1);
            works.First().WorkType.Should().Be(WorkType.Image);
        }
    }
}