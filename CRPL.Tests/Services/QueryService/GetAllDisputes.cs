using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
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
        await using (var context = new TestDbApplicationContextFactory().CreateContext(applications: new List<Application>
                     {
                         new DisputeApplication
                         {
                             Id = new Guid("21271541-2CC3-4456-BA1D-01BA1790B6A2"),
                             Created = DateTime.Now,
                             DisputeType = DisputeType.Usage,
                             Reason = "THIS IS A REASON"
                         }
                     }))
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var disputes = await queryService.GetAllDisputes(0);
            disputes.Should().NotBeNull();
            disputes.Should().NotContainNulls();
            disputes.Count.Should().Be(context.DisputeApplications.Count(x => x.Status == ApplicationStatus.Complete));
        }
    }
}