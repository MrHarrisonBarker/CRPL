using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PIPO.Verify.Test;

[TestFixture]
public class When_Verifying_Work
{
    [Test]
    public async Task Should_Be_Original()
    {
        var verifyService = new WorksVerificationService();

        var result = await verifyService.VerifyWork(Utils.LoadWorkIntoByteArray("./works/work3.jpg"));

        result.isOriginal.Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Be_Original()
    {
        var verifyService = new WorksVerificationService();
        
        var result = await verifyService.VerifyWork(Utils.LoadWorkIntoByteArray("./works/work1.jpg"));

        result.isOriginal.Should().BeFalse();
    }
}