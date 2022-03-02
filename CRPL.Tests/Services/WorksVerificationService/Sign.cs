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
        await using var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>()
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
        var (worksVerificationService, ipfsConnectionMock, cachedWorkRepository) = new WorksVerificationServiceFactory().Create(context);

        var signedWork = await worksVerificationService.Sign(context.RegisteredWorks.First());

        signedWork.Should().NotBeNull();
    }

    [Test]
    public async Task Should_Remove_Cached_Original_From_Repo()
    {
        await using var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>()
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
        {
            var (worksVerificationService, ipfsConnectionMock, cachedWorkRepository) = new WorksVerificationServiceFactory().Create(context);

            var signedWork = await worksVerificationService.Sign(context.RegisteredWorks.First());
            signedWork.Should().NotBeNull();

            await FluentActions.Invoking(async () => await worksVerificationService.Sign(context.RegisteredWorks.First()))
                .Should().ThrowAsync<Exception>().WithMessage("Cannot get cached work,**");
        }
    }

    [Test]
    public async Task Should_Add_File_To_Ipfs()
    {
        await using var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>
        {
            new()
            {
                Id = new Guid("97FDE5E0-4DBD-4EF1-94F9-279A29E64689"),
                Hash = new byte[] { 1, 1, 1, 1, 1 },
                Status = RegisteredWorkStatus.Registered,
                Created = DateTime.Now,
                Title = "Hello world",
                Registered = DateTime.Now
            }
        });
        var (worksVerificationService, ipfsConnectionMock, cachedWorkRepository) = new WorksVerificationServiceFactory().Create(context);

        var signedWork = await worksVerificationService.Sign(context.RegisteredWorks.First());

        signedWork.Cid.Should().BeEquivalentTo("QmcLgUi4YEbzD7heFAWkKPZDphhEicXwm7ER82nahvXdqQ");
        ipfsConnectionMock.Verify(x => x.AddFile(It.IsAny<MemoryStream>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task Should_Throw_When_Not_Registered()
    {
        await using var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>()
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
        var (worksVerificationService, ipfsConnectionMock, cachedWorkRepository) = new WorksVerificationServiceFactory().Create(context);

        await FluentActions.Invoking(async () => await worksVerificationService.Sign(context.RegisteredWorks.First()))
            .Should().ThrowAsync<WorkNotRegisteredException>();
    }
}