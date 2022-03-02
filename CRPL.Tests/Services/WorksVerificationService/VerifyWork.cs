using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.WorksVerificationService;

[TestFixture]
public class VerifyWork
{
    private List<RegisteredWork> Works;

    [SetUp]
    public async Task Setup()
    {
        Works = new List<RegisteredWork>
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
            },
            new()
            {
                Id = new Guid("EB2CC368-5731-432A-BCEA-D6838266C0E3"),
                Title = "Authentic work",
                Created = DateTime.Now,
                Status = RegisteredWorkStatus.ProcessingVerification,
                RightId = "2",
                RegisteredTransactionId = "TRANSACTION HASH",
                Hash = Encoding.UTF8.GetBytes("Authentic Work"),
                AssociatedApplication = new List<Application>
                {
                    new CopyrightRegistrationApplication
                    {
                        Id = Guid.NewGuid(),
                        Status = ApplicationStatus.Submitted
                    }
                }
            }
        };
    }

    [Test]
    public async Task Should_Be_Authentic()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        await worksVerificationServiceFactory.WorksVerificationService.VerifyWork(new Guid("EB2CC368-5731-432A-BCEA-D6838266C0E3"));

        var work = await dbFactory.Context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == new Guid("EB2CC368-5731-432A-BCEA-D6838266C0E3"));

        work.VerificationResult.IsAuthentic.Should().BeTrue();
        work.VerificationResult.Collision.Should().BeNull();
        work.Status.Should().Be(RegisteredWorkStatus.Verified);
    }

    [Test]
    public async Task Should_Not_Be_Authentic()
    {
        Works.Add(new RegisteredWork
        {
            Id = new Guid("60223CD9-07E5-46A7-BC26-C3AB5C710731"),
            Title = "Copied work",
            Created = DateTime.Now,
            Status = RegisteredWorkStatus.ProcessingVerification,
            RightId = "2",
            RegisteredTransactionId = "TRANSACTION HASH",
            Hash = Encoding.UTF8.GetBytes("Hello world"),
            AssociatedApplication = new List<Application>
            {
                new CopyrightRegistrationApplication
                {
                    Id = Guid.NewGuid(),
                    Status = ApplicationStatus.Submitted
                }
            }
        });

        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works);
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        await worksVerificationServiceFactory.WorksVerificationService.VerifyWork(new Guid("60223CD9-07E5-46A7-BC26-C3AB5C710731"));

        var work = await dbFactory.Context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == new Guid("60223CD9-07E5-46A7-BC26-C3AB5C710731"));

        work.VerificationResult.IsAuthentic.Should().BeFalse();
        work.VerificationResult.Collision.Should().NotBeNull();
        work.VerificationResult.Collision.Should().Be(Works.First().Id);
        work.Status.Should().Be(RegisteredWorkStatus.Rejected);
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var worksVerificationServiceFactory = new WorksVerificationServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await worksVerificationServiceFactory.WorksVerificationService.VerifyWork(Guid.Empty)).Should().ThrowAsync<WorkNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        
    }
}