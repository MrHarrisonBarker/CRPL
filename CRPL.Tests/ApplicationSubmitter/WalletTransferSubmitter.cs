using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using CRPL.Web.Services.Submitters;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.ApplicationSubmitter;

[TestFixture]
public class WalletTransferSubmitter
{
    [Test]
    public async Task Should_Submit_And_Transfer()
    {
        using var dbFactory = new TestDbApplicationContextFactory(applications: new List<Application>
        {
            new WalletTransferApplication
            {
                Id = new Guid("7E6C7A18-5EC8-4C35-8DBE-F8A71A0C2E92"),
                Created = DateTime.Now,
                Modified = DateTime.Now.AddDays(-1),
                Status = ApplicationStatus.Incomplete
            }
        });
        var serviceProviderFactory = new ServiceProviderWithContextFactory(dbFactory.Context);

        serviceProviderFactory.AccountManagementServiceMock.Setup(x => x.WalletTransfer(dbFactory.Context.WalletTransferApplications.First())).ReturnsAsync(dbFactory.Context.WalletTransferApplications.First());

        var submittedApplication = await dbFactory.Context.WalletTransferApplications.First().Submit(serviceProviderFactory.ServiceProviderMock.Object);

        submittedApplication.Status.Should().Be(ApplicationStatus.Submitted);
        
        serviceProviderFactory.AccountManagementServiceMock.Verify(x => x.WalletTransfer(submittedApplication), Times.Once);
    }
}