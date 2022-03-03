using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class AssignToApplication
{
    [TestFixture]
    public class UsingId
    {
        [Test]
        public async Task Should_Assign()
        {
            using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
            {
                new()
                {
                    Id = new Guid("F979870C-296D-45CF-B2E0-C7340631BCEB"),
                    Wallet = new() { PublicAddress = "ADDRESS" }
                }
            }, applications: new List<Application>
            {
                new DeleteAccountApplication
                {
                    Id = new Guid("4D905BF7-7F57-47FF-A1EA-426C8A23662D")
                }
            });
            var userServiceFactory = new UserServiceFactory(dbFactory.Context);

            userServiceFactory.UserService.AssignToApplication(new Guid("F979870C-296D-45CF-B2E0-C7340631BCEB"), new Guid("4D905BF7-7F57-47FF-A1EA-426C8A23662D"));

            dbFactory.Context.UserApplications.First().UserId.Should().Be(new Guid("F979870C-296D-45CF-B2E0-C7340631BCEB"));
            dbFactory.Context.UserApplications.Count().Should().Be(1);
        }

        [Test]
        public async Task Should_Throw_If_No_User()
        {
            using var dbFactory = new TestDbApplicationContextFactory();
            var userServiceFactory = new UserServiceFactory(dbFactory.Context);
            
            FluentActions.Invoking(() => userServiceFactory.UserService.AssignToApplication(Guid.Empty, Guid.Empty)).Should().Throw<UserNotFoundException>();
        }
    }


    [TestFixture]
    public class UsingAddress
    {
        [Test]
        public async Task Should_Assign_Using_Address()
        {
            using var dbFactory = new TestDbApplicationContextFactory(userAccounts: new List<UserAccount>
            {
                new()
                {
                    Id = new Guid("F979870C-296D-45CF-B2E0-C7340631BCEB"),
                    Wallet = new() { PublicAddress = "ADDRESS" }
                }
            }, applications: new List<Application>
            {
                new DeleteAccountApplication
                {
                    Id = new Guid("4D905BF7-7F57-47FF-A1EA-426C8A23662D")
                }
            });
            var userServiceFactory = new UserServiceFactory(dbFactory.Context);

            userServiceFactory.UserService.AssignToApplication("ADDRESS", new Guid("4D905BF7-7F57-47FF-A1EA-426C8A23662D"));

            dbFactory.Context.UserApplications.First().UserId.Should().Be(new Guid("F979870C-296D-45CF-B2E0-C7340631BCEB"));
            dbFactory.Context.UserApplications.Count().Should().Be(1);
        }
        
        [Test]
        public async Task Should_Throw_If_No_User()
        {
            using var dbFactory = new TestDbApplicationContextFactory();
            var userServiceFactory = new UserServiceFactory(dbFactory.Context);
            
            FluentActions.Invoking(() => userServiceFactory.UserService.AssignToApplication("", Guid.Empty)).Should().Throw<UserNotFoundException>();
        }
    }
}