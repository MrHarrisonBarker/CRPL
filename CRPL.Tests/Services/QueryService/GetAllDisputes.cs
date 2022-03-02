using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetAllDisputes
{
    [Test]
    public async Task Should_Get_All()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new DisputeApplication
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                DisputeType = DisputeType.Usage,
                Reason = "THIS IS A REASON",
                Status = ApplicationStatus.Submitted
            },
            new DisputeApplication
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                DisputeType = DisputeType.Usage,
                Reason = "THIS IS A REASON",
                Status = ApplicationStatus.Complete
            }
        });
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var disputes = await queryServiceFactory.QueryService.GetAllDisputes(0);
        disputes.Should().NotBeNull();
        disputes.Should().NotContainNulls();
        disputes.Count.Should().Be(dbFactory.Context.DisputeApplications.Count(x => x.Status == ApplicationStatus.Complete));
        disputes.Count.Should().BePositive();
    }
}