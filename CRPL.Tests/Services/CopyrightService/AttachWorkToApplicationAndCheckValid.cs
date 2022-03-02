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

namespace CRPL.Tests.Services.CopyrightService;

[TestFixture]
public class AttachWorkToApplicationAndCheckValid
{
    private List<RegisteredWork> Works;
    private List<Application> Applications;

    [SetUp]
    public async Task Setup()
    {
        Works = new List<RegisteredWork>()
        {
            new()
            {
                Hash = new byte[] { 0 },
                Id = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963"),
                RightId = "1",
                Title = "Hello world",
                Registered = DateTime.Now.AddDays(-1),
                Status = RegisteredWorkStatus.Registered
            },
            new()
            {
                Hash = new byte[] { 0, 0, 0 },
                Id = new Guid("85B77C6F-7D0C-4FCE-9691-4613C1F8BFDE"),
                RightId = "4",
                Title = "Created",
                Status = RegisteredWorkStatus.Created
            },
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

        Applications = new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI"
            },
            new CopyrightRegistrationApplication()
            {
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Id = new Guid("CDBEE1A0-D266-43AB-BB0A-16E3CD07451E"),
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;0x0000000000000000000000000000000000099992!50",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                Status = ApplicationStatus.Submitted,
                AssociatedWork = Works.First(),
            }
        };
    }

    [Test]
    public async Task Should_Attach_Work_To_Application()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        var workId = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963");
        var application = await dbFactory.Context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        await copyrightServiceFactory.CopyrightService.AttachWorkToApplicationAndCheckValid(workId, application);

        await dbFactory.Context.SaveChangesAsync();

        application.AssociatedWork.Should().NotBeNull();
        application.AssociatedWork.Id.Should().Be(workId);
    }

    [Test]
    public async Task Should_Not_Add_If_Already_Assigned()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        var workId = new Guid("9EE1AEF2-47BA-4A13-8AFE-693CF3D7E3DD");
        var application = await dbFactory.Context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("CDBEE1A0-D266-43AB-BB0A-16E3CD07451E"));

        await copyrightServiceFactory.CopyrightService.AttachWorkToApplicationAndCheckValid(workId, application);

        await dbFactory.Context.SaveChangesAsync();

        application.AssociatedWork.Should().NotBeNull();
        application.AssociatedWork.Id.Should().Be(workId);
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        var workId = Guid.Empty;
        var application = await dbFactory.Context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        await FluentActions.Invoking(async () => await copyrightServiceFactory.CopyrightService.AttachWorkToApplicationAndCheckValid(workId, application))
            .Should().ThrowAsync<WorkNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_Work_Not_Registered()
    {
        using var dbFactory = new TestDbApplicationContextFactory(registeredWorks: Works, applications: Applications);
        var copyrightServiceFactory = new CopyrightServiceFactory(dbFactory.Context);

        var workId = new Guid("85B77C6F-7D0C-4FCE-9691-4613C1F8BFDE");
        var application = await dbFactory.Context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        await FluentActions.Invoking(async () => await copyrightServiceFactory.CopyrightService.AttachWorkToApplicationAndCheckValid(workId, application))
            .Should().ThrowAsync<Exception>().WithMessage("The work is not registered!");
    }
}