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
public class SubmitTest
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

    [Test]
    public async Task Should_Throw_If_Already_Submitted()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await formsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(new Guid("57F0DC07-889D-446B-8E4D-D45DA4B4DCC4"))).Should()
                .ThrowAsync<Exception>().WithMessage("The application has already been submitted!");
        }
    }
    
    [Test]
    public async Task Should_Throw_If_Already_Complete()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            await FluentActions.Invoking(async () => await formsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(new Guid("807DADCF-9629-410D-8A36-4C366B5D53F5"))).Should()
                .ThrowAsync<Exception>().WithMessage("The application has already been complete!");
        }
    }
}