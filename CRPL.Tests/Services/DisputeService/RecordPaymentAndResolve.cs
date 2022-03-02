using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using NUnit.Framework;

namespace CRPL.Tests.Services.DisputeService;

[TestFixture]
public class RecordPaymentAndResolve
{
    [Test]
    public async Task Should_Record_Payment()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new DisputeApplication()
            {
                Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                Status = ApplicationStatus.Submitted,
                ExpectedRecourse = ExpectedRecourse.Payment
            }
        });

        var disputeServiceFactory = new DisputeServiceFactory(dbFactory.Context, new Dictionary<string, object>()
        {
            {
                "eth_getTransactionReceipt", new TransactionReceipt()
                {
                    TransactionHash = "HASH",
                    Status = new HexBigInteger(1)
                }
            }
        });

        await disputeServiceFactory.DisputeService.RecordPaymentAndResolve(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"), "HASH");

        var dispute = await dbFactory.Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"));

        dispute.ResolveResult.Should().NotBeNull();
        dispute.ResolveResult.Transaction.Should().BeEquivalentTo("HASH");
        dispute.ResolveResult.ResolvedStatus.Should().Be(ResolveStatus.Resolved);
    }

    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var disputeServiceFactory = new DisputeServiceFactory(dbFactory.Context);
        
        await FluentActions.Invoking(async () => await disputeServiceFactory.DisputeService.RecordPaymentAndResolve(Guid.Empty, ""))
            .Should().ThrowAsync<DisputeNotFoundException>();
    }
}