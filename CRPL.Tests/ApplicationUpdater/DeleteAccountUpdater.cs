using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Tests.Factories;
using CRPL.Web.Services.Updaters;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationUpdater;

[TestFixture]
public class DeleteAccountUpdater
{
    [Test]
    public async Task Should_Update()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new DeleteAccountApplication()
            {
                Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1)
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        var updatedApplication = await dbFactory.Context.DeleteAccountApplications.First().Update(new DeleteAccountInputModel
        {
            Id = new Guid("CC29C224-0F3D-48FA-A769-F72A56ADBAEF"),
            AccountId = new Guid("8729B942-B8A4-46D3-BCCB-9997C865FF20")
        });

        updatedApplication.AccountId.Should().Be(new Guid("8729B942-B8A4-46D3-BCCB-9997C865FF20"));
    }
}