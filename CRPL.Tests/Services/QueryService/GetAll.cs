using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;
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
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Hello world",
                WorkType = WorkType.Image,
                Created = DateTime.Now,
                Registered = DateTime.Now,
                Status = RegisteredWorkStatus.Registered
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Just made",
                WorkType = WorkType.Image,
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Created
            }
        });
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var works = await queryServiceFactory.QueryService.GetAll(0);
        works.Should().NotBeNull();
        works.Should().NotContainNulls();
        works.Count.Should().Be(dbFactory.Context.RegisteredWorks.Count(x => x.Status == RegisteredWorkStatus.Registered));
        works.Count.Should().BePositive();
    }
}