using System;
using System.Collections.Generic;
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
    private List<RegisteredWork> Works;

    [SetUp]
    public async Task SetUp()
    {
        Works = new List<RegisteredWork>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now.AddDays(-1),
                WorkType = WorkType.Image
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Another le Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now.AddDays(-2),
                WorkType = WorkType.Sound
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Another another Hello world title",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now.AddDays(-3),
                WorkType = WorkType.Video
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Another another another Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Created,
                WorkType = WorkType.PDF
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "yet Another another another Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.SentToChain,
                WorkType = WorkType.Image
            }
        };
    }

    [Test]
    public async Task Should_Search_Keyword()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var works = await queryServiceFactory.QueryService.Search(new StructuredQuery()
        {
            Keyword = "title"
        }, 0);

        works.Should().NotBeNull();
        works.Count.Should().BePositive();
        works.Count.Should().Be(1);
    }


    [Test]
    public async Task Should_Search_Sort_By()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var works = await queryServiceFactory.QueryService.Search(new StructuredQuery()
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

    [Test]
    public async Task Should_Only_Take_One()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var works = await queryServiceFactory.QueryService.Search(new StructuredQuery(), 0, 1);

        works.Should().NotBeNull();
        works.Count.Should().Be(1);
    }

    // [Test]
    // public async Task Should_Be_After()
    // {
    //     using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
    //     var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);
    //
    //     var works = await queryServiceFactory.QueryService.Search(new StructuredQuery
    //     {
    //         WorkFilters = new Dictionary<WorkFilter, string>
    //         {
    //             { WorkFilter.RegisteredAfter, DateTime.Now.ToString(CultureInfo.InvariantCulture) }
    //         }
    //     }, 0);
    //
    //     works.Should().NotBeNull();
    //     works.Count.Should().Be(0);
    // }
    //
    // [Test]
    // public async Task Should_Be_Before()
    // {
    //     using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
    //     var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);
    //
    //     var works = await queryServiceFactory.QueryService.Search(new StructuredQuery()
    //     {
    //         WorkFilters = new Dictionary<WorkFilter, string>
    //         {
    //             { WorkFilter.RegisteredBefore, DateTime.Now.AddDays(-2).AddHours(-10).ToString(CultureInfo.InvariantCulture) }
    //         }
    //     }, 0);
    //
    //     works.Should().NotBeNull();
    //     works.Count.Should().Be(1);
    // }

    [Test]
    public async Task Should_Be_Type()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var works = await queryServiceFactory.QueryService.Search(new StructuredQuery()
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