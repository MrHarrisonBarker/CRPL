using System;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using CRPL.Tests.Mocks;
using CRPL.Web.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.RegistrationService;

[TestFixture]
public class CompleteRegistration
{
    [Test]
    public async Task Should_Complete()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var mappings = MockWebUtils.DefaultMappings;
            mappings["eth_sendTransaction"] = "TEST TRANSACTION";

            var registrationService = new RegistrationServiceFactory().Create(context, mappings);

            var registeredWork = await registrationService.CompleteRegistration(new Guid("CDBEE1A0-D266-43AB-BB0A-16E3CD07451E"));

            registeredWork.Should().NotBeNull();
            registeredWork.RegisteredTransactionId.Should().BeEquivalentTo("TEST TRANSACTION");
        }
    }

    [Test]
    public async Task Should_Throw_If_No_Application()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var registrationService = new RegistrationServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await registrationService.CompleteRegistration(Guid.Empty))
                .Should().ThrowAsync<ApplicationNotFoundException>();
        }
    }

    [Test]
    public async Task Should_Throw_If_No_Associated_Work()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var registrationService = new RegistrationServiceFactory().Create(context, null);

            await FluentActions.Invoking(async () => await registrationService.CompleteRegistration(new Guid("0A47AF77-53E7-4CF1-B7DC-3B4E5E7D2C30")))
                .Should().ThrowAsync<Exception>().WithMessage("There is no work associated with this application!");
        }
    }
}