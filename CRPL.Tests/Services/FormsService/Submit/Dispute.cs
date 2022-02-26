using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService.Submit;

[TestFixture]
public class Dispute
{
    [Test]
    public async Task Should_Be_Submitted()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(applications: new List<Application>()
                     {
                         new DisputeApplication
                         {
                             Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                             Created = DateTime.Now,
                             Status = ApplicationStatus.Incomplete
                         }
                     }))
        {
            var formsService = new FormsServiceFactory().Create(context);

            var application = await formsService.Submit<DisputeApplication, DisputeViewModel>(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"));

            application.Should().NotBeNull();
            application.Status.Should().Be(ApplicationStatus.Submitted);
        }
    }
}