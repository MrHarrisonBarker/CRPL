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
public class AcceptRecourse
{
    [Test]
    public async Task Should_Update_Result()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(null, new List<Application>()
                     {
                         new DisputeApplication()
                         {
                             Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                             Status = ApplicationStatus.Submitted
                         }
                     }))
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);

            var dispute = await disputeService.AcceptRecourse(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"), "I accept this dispute");

            dispute.ResolveResult.Message.Should().BeEquivalentTo("I accept this dispute");
            dispute.ResolveResult.Rejected.Should().BeFalse();
            dispute.ResolveResult.Transaction.Should().BeNull();
            dispute.ResolveResult.ResolvedStatus.Should().Be(ResolveStatus.NeedsOnChainAction);
            dispute.Status.Should().Be(ApplicationStatus.Complete);
        }
    }

    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);
            
            await FluentActions.Invoking(async () => await disputeService.AcceptRecourse(Guid.Empty, ""))
                .Should().ThrowAsync<DisputeNotFoundException>();
        }
    }
}