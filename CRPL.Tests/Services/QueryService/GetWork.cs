using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using Moq;
using Nethereum.ABI.FunctionEncoding;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetWork
{
    [Test]
    public async Task Should_Get_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, null);

            var work = await queryService.GetWork(new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"));

            work.Should().NotBeNull();
            work.Title.Should().BeEquivalentTo("Hello world");
        }
    }

    [Test]
    public async Task Should_Send_To_Expired_Queue()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (queryService, connectionMock, contractRepoMock, expiryQueueMock) = new QueryServiceFactory().Create(context, new Dictionary<string, object>()
            {
                { "eth_call", new SmartContractRevertException("EXPIRED","") }
            });

            var workId = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963");
            var work = await queryService.GetWork(workId);

            work.Should().NotBeNull();
            expiryQueueMock.Verify(x => x.QueueExpire(workId), Times.Once);
        }
    }
}