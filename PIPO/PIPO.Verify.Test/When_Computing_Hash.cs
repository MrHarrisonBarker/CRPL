using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PIPO.Verify.Test;

[TestFixture]
public class When_Computing_Hash
{
    [Test]
    public async Task Should_Hash_Work()
    {
        var stream = Utils.LoadWorkIntoByteArray("./works/work1.jpg");
        
        var hash = VerifyHelpers.ComputeHash(stream);

        hash.Length.Should().BePositive();
    }

    [Test]
    public async Task Should_Hash_Same_Work_Same()
    {
        var work = Utils.LoadWorkIntoByteArray("./works/work1.jpg");
        
        var firstHash = VerifyHelpers.ComputeHash(work);
        var secondHash = VerifyHelpers.ComputeHash(work);

        firstHash.Should().Equal(secondHash);
        
        Console.WriteLine($"{Encoding.UTF8.GetString(firstHash)} == {Encoding.UTF8.GetString(secondHash)}");
    }
}