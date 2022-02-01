using System.IO;
using System.Text;
using System.Threading.Tasks;
using CRPL.Data.Works;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.WorkSigners;

[TestFixture]
public class SoundSigner
{
    [Test]
    public async Task Should_Add_Metadata_To_MP3()
    {
        var signer = new CRPL.Web.WorkSigners.SoundSigner("TEST SIGNATURE");

        var dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "WorkSigners/TestAssets/test.mp3");

        var file = await File.ReadAllBytesAsync(dir);

        var signedWork = signer.Sign(new CachedWork()
        {
            FileName = "test.mp3",
            Work = file,
            ContentType = "audio/mpeg"
        });

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
        
        var containsCopyright = Encoding.UTF8.GetString(signedWork.Work).Contains("TEST SIGNATURE");
        containsCopyright.Should().BeTrue();
    }
    
    [Test]
    public async Task Should_Add_Metadata_To_WAV()
    {
        var signer = new CRPL.Web.WorkSigners.SoundSigner("TEST SIGNATURE");

        var dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "WorkSigners/TestAssets/test.wav");

        var file = await File.ReadAllBytesAsync(dir);

        var signedWork = signer.Sign(new CachedWork()
        {
            FileName = "test.wav",
            Work = file,
            ContentType = "audio/wav"
        });

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
        //
        // copyrightTag.Should().NotBeNull();
        // copyrightTag.Description.Should().Contain("TEST SIGNATURE");
        
        var containsCopyright = Encoding.UTF8.GetString(signedWork.Work).Contains("TEST SIGNATURE");
        containsCopyright.Should().BeTrue();
    }
}