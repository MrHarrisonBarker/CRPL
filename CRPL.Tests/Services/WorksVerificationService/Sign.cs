using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.Services.WorksVerificationService;

[TestFixture]
public class Sign
{
    [Test]
    public async Task Should_Return_Signed_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("97FDE5E0-4DBD-4EF1-94F9-279A29E64689"),
                Hash = new byte[] { 1, 1, 1, 1, 1 },
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now,
                RegisteredTransactionId = "TRANSACTION",
                Created = DateTime.Now,
                Title = "Hello world",
                RightId = "1"
            }
        });
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        var signedWork = await worksVerificationServiceFactory.WorksVerificationService.Sign(dbFactory.Context.RegisteredWorks.First());

        signedWork.Should().NotBeNull();
    }

    [Test]
    public async Task Should_Remove_Cached_Original_From_Repo()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("97FDE5E0-4DBD-4EF1-94F9-279A29E64689"),
                Hash = new byte[] { 1, 1, 1, 1, 1 },
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now,
                RegisteredTransactionId = "TRANSACTION",
                Created = DateTime.Now,
                Title = "Hello world",
                RightId = "1"
            }
        });
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);


        var signedWork = await worksVerificationServiceFactory.WorksVerificationService.Sign(dbFactory.Context.RegisteredWorks.First());
        signedWork.Should().NotBeNull();

        worksVerificationServiceFactory.CachedWorkRepositoryMock.Setup(x => x.Get(It.IsAny<byte[]>())).Throws(new Exception("Cannot get cached work,"));

        await FluentActions.Invoking(async () => await worksVerificationServiceFactory.WorksVerificationService.Sign(dbFactory.Context.RegisteredWorks.First()))
            .Should().ThrowAsync<Exception>().WithMessage("Cannot get cached work,**");
    }

    [Test]
    public async Task Should_Add_File_To_Ipfs()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("97FDE5E0-4DBD-4EF1-94F9-279A29E64689"),
                Hash = new byte[] { 1, 1, 1, 1, 1 },
                Status = RegisteredWorkStatus.Registered,
                Registered = DateTime.Now,
                RegisteredTransactionId = "TRANSACTION",
                Created = DateTime.Now,
                Title = "Hello world",
                RightId = "1"
            }
        });
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        var signedWork = await worksVerificationServiceFactory.WorksVerificationService.Sign(dbFactory.Context.RegisteredWorks.First());

        signedWork.Cid.Should().BeEquivalentTo("QmcLgUi4YEbzD7heFAWkKPZDphhEicXwm7ER82nahvXdqQ");
        worksVerificationServiceFactory.IpfsConnectionMock.Verify(x => x.AddFile(It.IsAny<MemoryStream>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task Should_Throw_When_Not_Registered()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("97FDE5E0-4DBD-4EF1-94F9-279A29E64689"),
                Hash = new byte[] { 1, 1, 1, 1, 1 },
                Status = RegisteredWorkStatus.Rejected,
                Created = DateTime.Now,
                Title = "Hello world"
            }
        });
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);
        
        await FluentActions.Invoking(async () => await worksVerificationServiceFactory.WorksVerificationService.Sign(dbFactory.Context.RegisteredWorks.First()))
            .Should().ThrowAsync<WorkNotRegisteredException>();
    }
}