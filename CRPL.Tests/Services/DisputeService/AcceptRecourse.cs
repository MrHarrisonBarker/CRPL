using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
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
    private List<RegisteredWork> Works;
    private List<Application> Applications;

    [SetUp]
    public async Task SetUp()
    {
        Works = new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("0FB0C1C0-B3C6-4C1B-88BE-9DCC53D4DAA5"),
                Title = "Hello world",
                Status = RegisteredWorkStatus.Registered,
                Created = DateTime.Now,
                Registered = DateTime.Now,
                RightId = "1"
            }
        };

        Applications = new List<Application>
        {
            new DisputeApplication
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                Status = ApplicationStatus.Submitted,
                ExpectedRecourse = ExpectedRecourse.Payment,
                AssociatedWork = Works.First()
            }
        };
    }

    [Test]
    public async Task Should_Update_Result()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, Applications))
        {
            var (disputeService, formsServiceMock) = new DisputeServiceFactory().Create(context, null);

            var dispute = await disputeService.AcceptRecourse(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"), "I accept this dispute");

            dispute.ResolveResult.Message.Should().BeEquivalentTo("I accept this dispute");
            dispute.ResolveResult.Rejected.Should().BeFalse();
            dispute.ResolveResult.Transaction.Should().BeNull();
            dispute.ResolveResult.ResolvedStatus.Should().Be(ResolveStatus.NeedsOnChainAction);
            dispute.Status.Should().Be(ApplicationStatus.Submitted);
        }
    }

    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (disputeService, formsServiceMock) = new DisputeServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await disputeService.AcceptRecourse(Guid.Empty, ""))
                .Should().ThrowAsync<DisputeNotFoundException>();
        }
    }
}