using System.IO;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Works;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.WorkSigners;

[TestFixture]
public class TextSigner
{
    [Test]
    public async Task Should_Add_Metadata_To_PDF()
    {
        var signer = new CRPL.Web.WorkSigners.TextSigner("TEST SIGNATURE");

        var dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "WorkSigners/TestAssets/test.pdf");

        var file = await File.ReadAllBytesAsync(dir);

        var signedWork = signer.Sign(new CachedWork()
        {
            FileName = "test.pdf",
            Work = file,
            ContentType = "application/pdf"
        });
        
        var containsCopyright = Encoding.UTF8.GetString(signedWork.Work).Contains("TEST SIGNATURE");
        containsCopyright.Should().BeTrue();
    }
}