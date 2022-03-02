using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.RegistrationService;

[TestFixture]
public class StartRegistration
{
    [Test]
    public async Task Should_Start_Registration()
    {
        using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>()
        {
            new()
            {
               Id = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"),
               Status = UserAccount.AccountStatus.Complete,
               Wallet = new UserWallet {PublicAddress = "0x0000000000000000000000000000000000099991"}
            }
        }, applications: new List<Application>
        {
            new CopyrightRegistrationApplication
            {
                Id = new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30"),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                ApplicationType = ApplicationType.CopyrightRegistration,
                OwnershipStakes = "0x0000000000000000000000000000000000099991!50;",
                Legal = "LEGAL META",
                Title = "HELLO WORLD",
                WorkHash = Encoding.UTF8.GetBytes("HASH"),
                WorkUri = "URI",
                AssociatedUsers = new List<UserApplication>
                {
                    new() { UserId = new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C") }
                },
            }
        });
        var registrationServiceFactory = new RegistrationServiceFactory(dbFactory.Context);

        var registeredWork = await registrationServiceFactory.RegistrationService.StartRegistration(dbFactory.Context.CopyrightRegistrationApplications.First());

        registeredWork.Should().NotBeNull();
        registeredWork.Title.Should().BeEquivalentTo("Hello world");
        registeredWork.UserWorks.Should().NotBeNull();
        registeredWork.UserWorks.Count.Should().Be(1);
    }
}