using System;
using System.Text;
using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.WorksVerificationService;

[TestFixture]
public class Sign
{
    [Test]
    public async Task Should_Return_Signed_Work()
    {
        await using var context = new TestDbApplicationContextFactory().CreateContext();
        var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

        var signedWork = worksVerificationService.Sign(new byte[] { 0 }, "TEST SIG");

        signedWork.Should().NotBeNull();
        var decodedString = Encoding.UTF8.GetString(signedWork.Work);
        decodedString.Should().Contain("TEST SIG");
    }

    [Test]
    public async Task Should_Throw_When_Not_Found()
    {
        await using var context = new TestDbApplicationContextFactory().CreateContext();
        var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

        await FluentActions.Invoking(async () => worksVerificationService.Sign(new byte[] { 1, 1, 1, 1 }, "TEST SIG"))
            .Should().ThrowAsync<Exception>().WithMessage($"File already exists, {Convert.ToBase64String(new byte[] { 1, 1, 1, 1 })}");
    }
}