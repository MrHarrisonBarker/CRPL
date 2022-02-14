using System;
using System.Threading.Tasks;
using CRPL.Data.Proposal;
using CRPL.Tests.Factories;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.CopyrightService;

[TestFixture]
public class BindProposal
{
    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var copyrightService = new CopyrightServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await copyrightService.BindProposal(new BindProposalInput() { ApplicationId = Guid.Empty }))
                .Should().ThrowAsync<ApplicationNotFoundException>();
        }
    }
    
    [Test]
    public async Task Should_Throw_If_No_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var copyrightService = new CopyrightServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await copyrightService.BindProposal(new BindProposalWorkInput() { WorkId = Guid.Empty }))
                .Should().ThrowAsync<WorkNotFoundException>();
        }
    }
}