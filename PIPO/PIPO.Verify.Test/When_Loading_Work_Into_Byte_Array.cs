using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PIPO.Verify.Test;

[TestFixture]
public class When_Loading_Work_Into_Byte_Array
{
    [Test]
    public async Task Should_Be_Byte_Array()
    {
        var bytes = Utils.LoadWorkIntoByteArray("./works/work1.jpg");
        
        bytes.Should().NotBeNull();
        bytes.Length.Should().Be(844430);
    }
}