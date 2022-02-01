using System.IO;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Works;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.WorkSigners;

[TestFixture]
public class VideoSigner
{
    [Test]
    public async Task Should_Add_Metadata_To_MP4()
    {
        var signer = new CRPL.Web.WorkSigners.VideoSigner("TEST SIGNATURE");

        var dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "WorkSigners/TestAssets/test.mp4");

        var file = await File.ReadAllBytesAsync(dir);

        var signedWork = signer.Sign(new CachedWork()
        {
            Work = file,
            FileName = "test.mp4",
            ContentType = "video/mp4"
        });

        var containsCopyright = Encoding.UTF8.GetString(signedWork.Work).Contains("TEST SIGNATURE");
        
        // IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(new MemoryStream(signedWork.Work)).ToList();
        //
        // Tag? copyrightTag = null;
        // foreach (var directory in directories)
        // {
        //     foreach (var tag in directory.Tags)
        //     {
        //         if (tag.Name == "Copyright") copyrightTag = tag;
        //     }
        // }

        // copyrightTag.Should().NotBeNull();
        // copyrightTag.Description.Should().Contain("TEST SIGNATURE");
        containsCopyright.Should().BeTrue();
    }
}