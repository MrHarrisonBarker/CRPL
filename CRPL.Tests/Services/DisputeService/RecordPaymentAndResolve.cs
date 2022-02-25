using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        await using (var context = new TestDbApplicationContextFactory().CreateContext(applications: new List<Application>()
                     {
                         new DisputeApplication()
                         {
                             Id = new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"),
                             Status = ApplicationStatus.Submitted
                         }
                     }))
        {
            var disputeService = new DisputeServiceFactory().Create(context, new Dictionary<string, object>()
            {
                {
                    "eth_getTransactionReceipt", new TransactionReceipt()
                    {
                        TransactionHash = "HASH",
                        Status = new HexBigInteger(1)
                    }
                }
            });

            await disputeService.RecordPaymentAndResolve(new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"), "HASH");

            var dispute = await context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == new Guid("DB27D402-B34E-42AE-AC6E-054AF46EB04A"));

            dispute.ResolveResult.Should().NotBeNull();
            dispute.ResolveResult.Transaction.Should().BeEquivalentTo("HASH");
            dispute.ResolveResult.ResolvedStatus.Should().Be(ResolveStatus.Resolved);
        }
    }
    
    [Test]
    public async Task Should_Throw_When_No_Dispute()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var disputeService = new DisputeServiceFactory().Create(context, null);
            
            await FluentActions.Invoking(async () => await disputeService.RecordPaymentAndResolve(Guid.Empty, ""))
                .Should().ThrowAsync<DisputeNotFoundException>();
        }
    }
}