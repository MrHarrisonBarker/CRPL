using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using FluentAssertions;
using Moq;
using Nethereum.ABI.FunctionEncoding;
using NUnit.Framework;

namespace CRPL.Tests.Services.QueryService;

[TestFixture]
public class GetWork
{
    [Test][Ignore("overflow error")]
    public async Task Should_Get_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                RightId = "1"
            }
        });
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context);

        var work = await queryServiceFactory.QueryService.GetWork(new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"));

        work.Should().NotBeNull();
        work.Title.Should().BeEquivalentTo("Hello world");
    }

    [Test]
    public async Task Should_Send_To_Expired_Queue()
    {
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_call"] = new SmartContractRevertException("EXPIRED", "");
        
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"),
                Title = "Hello world",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.Registered,
                RightId = "1"
            }
        });
        var queryServiceFactory = new QueryServiceFactory(dbFactory.Context, mappings);

        var workId = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963");
        var work = await queryServiceFactory.QueryService.GetWork(workId);

        work.Should().NotBeNull();
        queryServiceFactory.ExpiryQueueMock.Verify(x => x.QueueExpire(workId), Times.Once);
    }
}