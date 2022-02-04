using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRPL.Data.Applications.ViewModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.FormsService;

[TestFixture]
public class GetMyApplications
{
    [Test]
    public async Task Should_Get_Users_Applications()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var formsService = new FormsServiceFactory().Create(context);

            var applications = await formsService.GetMyApplications(new Guid("A9B73346-DA66-4BD5-97FE-0A0113E52D4C"));

            applications.Count.Should().BePositive();
            applications.Should().BeOfType<List<ApplicationViewModel>>();
        }
    }
}