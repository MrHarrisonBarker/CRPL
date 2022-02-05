using System;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class Submit
{
    [Test]
    public async Task Should_Be_Submitted()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var application = await formsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

            application.Should().NotBeNull();
            application.Status.Should().Be(ApplicationStatus.Submitted);
        }
    }

    [Test]
    public async Task Should_Throw_If_Not_Found()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await formsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(Guid.Empty)).Should()
                .ThrowAsync<ApplicationNotFoundException>();
        }
    }
}