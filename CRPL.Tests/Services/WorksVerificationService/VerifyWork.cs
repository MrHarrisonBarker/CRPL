using System.Threading.Tasks;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.WorksVerificationService;

[TestFixture]
public class VerifyWork
{
    [Test]
    public async Task Should_Be_Authentic()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

            var result = await worksVerificationService.VerifyWork(new byte[] { 1, 1, 1 });

            result.IsAuthentic.Should().BeTrue();
            result.Collisions.Count.Should().Be(0);
        }
    }

    [Test]
    public async Task Should_Not_Be_Authentic()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var worksVerificationService = new WorksVerificationServiceFactory().Create(context);

            var result = await worksVerificationService.VerifyWork(new byte[] { 0 });

            result.IsAuthentic.Should().BeFalse();
            result.Collisions.Count.Should().Be(1);
        }
    }
}