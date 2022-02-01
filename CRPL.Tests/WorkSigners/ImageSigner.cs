using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Works;
using FluentAssertions;
using MetadataExtractor;
using NUnit.Framework;
using Directory = MetadataExtractor.Directory;

namespace CRPL.Tests.WorkSigners;

[TestFixture]
public class ImageSigner
{
    [Test]
    public async Task Should_Add_Metadata_To_JPG()
    {
        var signer = new CRPL.Web.WorkSigners.ImageSigner("TEST SIGNATURE");

        var dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "WorkSigners/TestAssets/test.jpg");

        var file = await File.ReadAllBytesAsync(dir);

        var signedWork = signer.Sign(new CachedWork()
        {
            Work = file
        });

        IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(new MemoryStream(signedWork.Work)).ToList();
        
        Tag? copyrightTag = null;
        foreach (var directory in directories)
        {
            foreach (var tag in directory.Tags)
            {
                if (tag.Name == "Copyright") copyrightTag = tag;
            }
        }

        copyrightTag.Should().NotBeNull();
        copyrightTag.Description.Should().Contain("TEST SIGNATURE");
    }
}