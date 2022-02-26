using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.CopyrightService;

[TestFixture]
public class AttachWorkToApplicationAndCheckValid
{
    [Test]
    public async Task Should_Attach_Work_To_Application()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, null);

            var workId = new Guid("D54F35CC-3C8A-471C-A641-2BB5A59A8963");
            var application = await context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

            await copyrightService.AttachWorkToApplicationAndCheckValid(workId, application);

            await context.SaveChangesAsync();

            application.AssociatedWork.Should().NotBeNull();
            application.AssociatedWork.Id.Should().Be(workId);
        }
    }

    [Test]
    public async Task Should_Not_Add_If_Already_Assigned()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, null);

            var workId = new Guid("9EE1AEF2-47BA-4A13-8AFE-693CF3D7E3DD");
            var application = await context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("CDBEE1A0-D266-43AB-BB0A-16E3CD07451E"));

            await copyrightService.AttachWorkToApplicationAndCheckValid(workId, application);

            await context.SaveChangesAsync();

            application.AssociatedWork.Should().NotBeNull();
            application.AssociatedWork.Id.Should().Be(workId);
        }
    }

    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, null);

            var workId = Guid.Empty;
            var application = await context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

            await FluentActions.Invoking(async () => await copyrightService.AttachWorkToApplicationAndCheckValid(workId, application))
                .Should().ThrowAsync<WorkNotFoundException>();
        }
    }

    [Test]
    public async Task Should_Throw_If_Work_Not_Registered()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, null);

            var workId = new Guid("85B77C6F-7D0C-4FCE-9691-4613C1F8BFDE");
            var application = await context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

            await FluentActions.Invoking(async () => await copyrightService.AttachWorkToApplicationAndCheckValid(workId, application))
                .Should().ThrowAsync<Exception>().WithMessage("The work is not registered!");
        }
    }
}