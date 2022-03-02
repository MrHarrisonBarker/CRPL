using System;
using System.Collections.Generic;
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
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        var application = await formsServiceFactory.FormsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

        application.Should().NotBeNull();
        application.Status.Should().Be(ApplicationStatus.Submitted);
    }

    [Test]
    public async Task Should_Throw_If_Not_Found()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await formsServiceFactory.FormsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(Guid.Empty)).Should()
            .ThrowAsync<ApplicationNotFoundException>();
    }

    [Test]
    public async Task Should_Throw_If_Already_Submitted()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1),
                Status = ApplicationStatus.Submitted
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () =>
                await formsServiceFactory.FormsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30")))
            .Should()
            .ThrowAsync<Exception>().WithMessage("The application has already been submitted!");
    }

    [Test]
    public async Task Should_Throw_If_Already_Complete()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1),
                Status = ApplicationStatus.Complete
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () =>
                await formsServiceFactory.FormsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30")))
            .Should()
            .ThrowAsync<Exception>().WithMessage("The application has already been complete!");
    }
}