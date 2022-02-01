using System.IO;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Works;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.WorkSigners;

[TestFixture]
public class UniversalSigner
{
    [Test]
    public async Task Should_Add_Signature()
    {
        var signer = new CRPL.Web.WorkSigners.UniversalSigner("TEST SIGNATURE");

        var dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "WorkSigners/TestAssets/test.jpg");

        var file = await File.ReadAllBytesAsync(dir);
        
        var signedWork = signer.Sign(new CachedWork()
        {
            Work = file
        });
        
        var containsCopyright = Encoding.UTF8.GetString(signedWork.Work).Contains("TEST SIGNATURE");
        containsCopyright.Should().BeTrue();
    }
}