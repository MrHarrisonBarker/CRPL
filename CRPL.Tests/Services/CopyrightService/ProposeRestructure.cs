using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
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
    private List<Application> Applications;
    private List<RegisteredWork> Works;

    [SetUp]
    public async Task SetUp()
    {
        Works = new List<RegisteredWork>()
        {
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("9EE1AEF2-47BA-4A13-8AFE-693CF3D7E3DD"),
                RightId = "6",
                Title = "Assigned",
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now.AddDays(-1),
                RegisteredTransactionId = "TRANSACTION HASH"
            }
        };

        Applications = new List<Application>
        {
            new OwnershipRestructureApplication
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"),
                ApplicationType = ApplicationType.OwnershipRestructure,
                CurrentStructure = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                ProposedStructure = "0x0000000000000000000000000000000000099991!90;0x0000000000000000000000000000000000099992!10",
                Status = ApplicationStatus.Submitted,
                AssociatedWork = Works.First()
            }
        };
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await copyrightServiceFactory.CopyrightService.ProposeRestructure(new OwnershipRestructureApplication()
            {
                AssociatedWork = null
            }))
            .Should().ThrowAsync<WorkNotFoundException>();
    }

    [Test]
    public async Task Should_Send_Transaction()
    {
        var mappings = MockWebUtils.DefaultMappings;
        mappings["eth_sendTransaction"] = "TEST TRANSACTION";

        using var dbFactory = new TestDbApplicationContextFactory(applications: Applications, registeredWorks: Works);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context, mappings);

        var proposed = await copyrightServiceFactory.CopyrightService.ProposeRestructure(dbFactory.Context.OwnershipRestructureApplications.First());

        proposed.Should().NotBeNull();
        proposed.TransactionId.Should().BeEquivalentTo("TEST TRANSACTION");
    }

    [Test]
    public async Task Should_Send_To_Expired_Queue()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: Applications, registeredWorks: Works);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context, new Dictionary<string, object>()
        {
            { "eth_call", new SmartContractRevertException("EXPIRED", "") }
        });

        var application = await dbFactory.Context.OwnershipRestructureApplications
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.Id == new Guid("39E52B21-5BA4-4F69-AFF8-28294391EFB8"));

        await FluentActions.Invoking(async () => await copyrightServiceFactory.CopyrightService.ProposeRestructure(application)).Should().ThrowAsync<WorkExpiredException>();

        copyrightServiceFactory.ExpiryQueueMock.Verify(x => x.QueueExpire(application.AssociatedWork.Id), Times.Once);
    }
}