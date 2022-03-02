using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.DisputeService;

[TestFixture]
public class RejectRecourse
{
    [Test]
    public async Task Should_Update_Result()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new DisputeApplication()
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                Status = ApplicationStatus.Submitted
            }
        });
        var disputeServiceFactory = new DisputeServiceFactory(dbFactory.Context);

        var dispute = await disputeServiceFactory.DisputeService.RejectRecourse(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"), "I reject this dispute");

        dispute.ResolveResult.Message.Should().BeEquivalentTo("I reject this dispute");
        dispute.ResolveResult.Rejected.Should().BeTrue();
        dispute.ResolveResult.Transaction.Should().BeNull();
        dispute.ResolveResult.ResolvedStatus.Should().Be(ResolveStatus.Resolved);
    }

    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var disputeServiceFactory = new DisputeServiceFactory(dbFactory.Context);
        
        await FluentActions.Invoking(async () => await disputeServiceFactory.DisputeService.RejectRecourse(Guid.Empty, ""))
            .Should().ThrowAsync<DisputeNotFoundException>();
    }
}