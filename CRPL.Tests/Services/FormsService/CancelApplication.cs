using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class CancelApplication
{
    [Test]
    public async Task Should_Cancel()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            await formsService.CancelApplication(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

            (await context.Applications.FirstOrDefaultAsync(x => x.Id == new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"))).Should().BeNull();
        }
    }

    [Test]
    public async Task Should_Throw_When_Not_Found()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await formsService.CancelApplication(Guid.Empty)).Should().ThrowAsync<ApplicationNotFoundException>();
        }
    }
}