using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
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
        await using (var context = new TestDbApplicationContextFactory().CreateContext(null, new List<Application>()
                     {
                         new DisputeApplication()
                         {
                             Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                             
                         }
                     }))
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);

            var dispute = await disputeService.RejectRecourse(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"), "I reject this dispute");

            dispute.ResolveResult.Message.Should().BeEquivalentTo("I reject this dispute");
            dispute.ResolveResult.Rejected.Should().BeTrue();
            dispute.ResolveResult.Transaction.Should().BeNull();
        }
    }

    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);
            
            await FluentActions.Invoking(async () => await disputeService.RejectRecourse(Guid.Empty, ""))
                .Should().ThrowAsync<DisputeNotFoundException>();
        }
    }
}