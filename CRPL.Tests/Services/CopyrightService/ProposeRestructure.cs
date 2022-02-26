using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nethereum.ABI.FunctionEncoding;
using NUnit.Framework;

namespace CRPL.Tests.Services.CopyrightService;

[TestFixture]
public class ProposeRestructure
{
    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await copyrightService.ProposeRestructure(new OwnershipRestructureApplication()
                {
                    AssociatedWork = null
                }))
                .Should().ThrowAsync<WorkNotFoundException>();
        }
    }

    [Test]
    public async Task Should_Send_Transaction()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_sendTransaction"] = "TEST TRANSACTION";
            
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, mappings);

            var application = await context.OwnershipRestructureApplications
                .Include(x => x.AssociatedWork)
                .FirstOrDefaultAsync(x => x.Id == new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"));

            var proposed = await copyrightService.ProposeRestructure(application);

            proposed.Should().NotBeNull();
            proposed.TransactionId.Should().BeEquivalentTo("TEST TRANSACTION");
        }
    }

    [Test]
    public async Task Should_Send_To_Expired_Queue()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, new Dictionary<string, object>()
            {
                { "eth_call", new SmartContractRevertException("EXPIRED","") }
            });

            var application = await context.OwnershipRestructureApplications
                .Include(x => x.AssociatedWork)
                .FirstOrDefaultAsync(x => x.Id == new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"));

            await FluentActions.Invoking(async () => await copyrightService.ProposeRestructure(application)).Should().ThrowAsync<WorkExpiredException>();
            
            expiryQueueMock.Verify(x => x.QueueExpire(application.AssociatedWork.Id), Times.Once);        
        }
    }
}