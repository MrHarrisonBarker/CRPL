using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PIPO.Verify.Test.works;

[TestFixture]
public class When_Comparing_Works
{
    [Test]
    public async Task Should_Be_The_Same()
    {
        var result = VerifyHelpers.CompareWork(Utils.LoadWorkIntoByteArray("./works/work1.jpg"), Utils.LoadWorkIntoByteArray("./works/work1.jpg"));

        result.Should().BeTrue();
    }

    [Test]
    public async Task Should_Not_Be_The_Same()
    {
        var result = VerifyHelpers.CompareWork(Utils.LoadWorkIntoByteArray("./works/work2.jpg"), Utils.LoadWorkIntoByteArray("./works/work1.jpg"));

        result.Should().BeFalse();
    }
}