using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.WorksVerificationService;

[TestFixture]
public class VerifyWork
{
    private static readonly List<RegisteredWork> Works = new()
    {
        new()
        {
            Id = new Guid("C714A94E-BE61-4D7B-A4CE-28F0667FAEAD"),
            Title = "Hello world",
            Created = DateTime.Now,
            Status = RegisteredWorkStatus.ProcessingVerification,
            RightId = "1",
            RegisteredTransactionId = "TRANSACTION HASH",
            Hash = Encoding.UTF8.GetBytes("Hello world")
        }
    };
    
    [Test]
    public async Task Should_Be_Authentic()
    {
        Works.Add(new RegisteredWork()
        {
            Id = new Guid("EB2CC368-5731-432A-BCEA-D6838266C0E3"),
            Title = "Authentic work",
            Created = DateTime.Now,
            Status = RegisteredWorkStatus.ProcessingVerification,
            RightId = "2",
            RegisteredTransactionId = "TRANSACTION HASH",
            Hash = Encoding.UTF8.GetBytes("Authentic Work")
        });
        
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, new List<Application>()))
        {
            var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

            await worksVerificationService.VerifyWork(Works.Last().Id);

            var work = await context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == Works.Last().Id);

            work.VerificationResult.IsAuthentic.Should().BeTrue();
            work.VerificationResult.Collision.Should().BeNull();
            work.Status.Should().Be(RegisteredWorkStatus.Verified);
        }
    }

    [Test]
    public async Task Should_Not_Be_Authentic()
    {
        Works.Add(new RegisteredWork()
        {
            Id = new Guid("EB2CC368-5731-432A-BCEA-D6838266C0E3"),
            Title = "Copied work",
            Created = DateTime.Now,
            Status = RegisteredWorkStatus.ProcessingVerification,
            RightId = "2",
            RegisteredTransactionId = "TRANSACTION HASH",
            Hash = Encoding.UTF8.GetBytes("Hello world")
        });
        
        await using (var context = new TestDbApplicationContextFactory().CreateContext(Works, new List<Application>()))
        {
            var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

            await worksVerificationService.VerifyWork(Works.Last().Id);

            var work = await context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == Works.Last().Id);

            work.VerificationResult.IsAuthentic.Should().BeFalse();
            work.VerificationResult.Collision.Should().NotBeNull();
            work.VerificationResult.Collision.Should().Be(Works.First().Id);
            work.Status.Should().Be(RegisteredWorkStatus.Rejected);
        }
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await worksVerificationService.VerifyWork(Guid.Empty)).Should().ThrowAsync<WorkNotFoundException>();
        }
    }
}