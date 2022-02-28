using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService.Update;

[TestFixture]
public class Dispute
{
    [Test]
    public async Task Should_Update()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>(), new List<Application>()
                     {
                         new DisputeApplication
                         {
                             Id = new Guid("EF088FBE-D33E-40FD-A37F-69EFCB7E3ED7"),
                             Created = DateTime.Now,
                             Infractions = 1,
                             Reason = "This is a reason",
                             Spotted = DateTime.Now.AddDays(-1),
                             Status = ApplicationStatus.Incomplete,
                             ContactAddress = "ADDRESS",
                             DisputeType = DisputeType.Ownership,
                             LinkToInfraction = "LINK",
                             ExpectedRecourse = ExpectedRecourse.ChangeOfOwnership,
                             ExpectedRecourseData = "GET RID OF IT"
                         }
                     }, new List<UserAccount>()))
        {
            var (formsService, userServiceMock)  = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<DisputeViewModel>(new DisputeInputModel()
            {
                Id = new Guid("EF088FBE-D33E-40FD-A37F-69EFCB7E3ED7"),
                DisputeType = DisputeType.Usage,
                Reason = "This is a new reason",
                Infractions = 10,
                Spotted = DateTime.MinValue,
                LinkToInfraction = "NEW LINK",
                ExpectedRecourse = ExpectedRecourse.Payment,
                ExpectedRecourseData = "100",
                ContactAddress = "NEW ADDRESS",
            });

            updatedApplication.Reason.Should().BeEquivalentTo("This is a new reason");
            updatedApplication.DisputeType.Should().Be(DisputeType.Usage);
            updatedApplication.Infractions.Should().Be(10);
            updatedApplication.Spotted.Should().Be(DateTime.MinValue);
            updatedApplication.LinkToInfraction.Should().BeEquivalentTo("NEW LINK");
            updatedApplication.ExpectedRecourse.Should().Be(ExpectedRecourse.Payment);
            updatedApplication.ExpectedRecourseData.Should().BeEquivalentTo("100");
            updatedApplication.ContactAddress.Should().BeEquivalentTo("NEW ADDRESS");
        }
    }

    [Test]
    public async Task Should_Create_New()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>(), new List<Application>(), new List<UserAccount>()))
        {
            var (formsService, userServiceMock)  = new FormsServiceFactory().Create(context);

            var updatedApplication = await formsService.Update<DisputeViewModel>(new DisputeInputModel()
            {
                DisputeType = DisputeType.Usage,
                Reason = "This is a reason"
            });

            updatedApplication.Reason.Should().Be("This is a reason");
            updatedApplication.DisputeType.Should().Be(DisputeType.Usage);
        }
    }

    [Test]
    public async Task Should_Assign_User_And_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext(new List<RegisteredWork>
                     {
                         new()
                         {
                             Id = new Guid("8B0750C1-9FB6-4A1D-ABA0-41C581E59753"),
                             Title = "Hello world",
                             Status = RegisteredWorkStatus.Registered,
                             Registered = DateTime.Now
                         }
                     }, new List<Application>(), new List<UserAccount>()
                     {
                         new()
                         {
                             Id = new Guid("3B26A0BF-B393-4703-9158-4EEDACB943AC"),
                             Wallet = new UserWallet() { PublicAddress = TestConstants.TestAccountAddress }
                         }
                     }))
        {
            var (formsService, userServiceMock)  = new FormsServiceFactory().Create(context);

            await formsService.Update<DisputeViewModel>(new DisputeInputModel()
            {
                DisputeType = DisputeType.Usage,
                Reason = "This is a reason",
                DisputedWorkId = new Guid("8B0750C1-9FB6-4A1D-ABA0-41C581E59753"),
                AccuserId = new Guid("3B26A0BF-B393-4703-9158-4EEDACB943AC")
            });

            var updatedApplication = context.DisputeApplications
                .Include(x => x.AssociatedWork)
                .Include(x => x.AssociatedUsers)
                .ThenInclude(x => x.UserAccount)
                .FirstOrDefault();
            
            updatedApplication.Reason.Should().Be("This is a reason");
            updatedApplication.DisputeType.Should().Be(DisputeType.Usage);
            updatedApplication.AssociatedWork.Id.Should().Be(new Guid("8B0750C1-9FB6-4A1D-ABA0-41C581E59753"));
            updatedApplication.AssociatedUsers.First().UserAccount.Id.Should().Be(new Guid("3B26A0BF-B393-4703-9158-4EEDACB943AC"));
        }
    }
}