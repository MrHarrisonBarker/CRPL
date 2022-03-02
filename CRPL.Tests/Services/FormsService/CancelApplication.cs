using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class CancelApplication
{
    [Test]
    public async Task Should_Cancel()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>()
        {
            new CopyrightRegistrationApplication()
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        await formsServiceFactory.FormsService.CancelApplication(new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"));

        dbFactory.Context.Applications.Any().Should().BeFalse();
    }

    [Test]
    public async Task Should_Throw_When_Not_Found()
    {
        using var dbFactory = new TestDbApplicationContextFactory();
        var formsServiceFactory = new FormsServiceFactory(dbFactory.Context);

        await FluentActions.Invoking(async () => await formsServiceFactory.FormsService.CancelApplication(Guid.Empty)).Should().ThrowAsync<ApplicationNotFoundException>();
    }
}