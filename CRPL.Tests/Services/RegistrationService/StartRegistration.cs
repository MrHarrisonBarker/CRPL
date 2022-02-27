using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CRPL.Tests.Services.RegistrationService;

[TestFixture]
public class StartRegistration
{
    [Test]
    public async Task Should_Start_Registration()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var registrationService = new RegistrationServiceFactory().Create(context, null);

            var user = await context.UserAccounts.FirstAsync();

            var registeredWork = await registrationService.StartRegistration(new CopyrightRegistrationApplication()
            {
                WorkHash = new byte[] { 0, 0, 1 },
                Title = "Hello world",
                AssociatedUsers = new List<UserApplication>()
                {
                    new() {UserAccount = user}
                }
            });

            registeredWork.Should().NotBeNull();
            registeredWork.Title.Should().BeEquivalentTo("Hello world");
            registeredWork.UserWorks.Should().NotBeNull();
            registeredWork.UserWorks.Count.Should().Be(1);
        }
    }
}