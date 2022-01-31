using CRPL.Data.Works;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace CRPL.Web.WorkSigners;

public class ImageSigner : IWorkSigner
{
    public CachedWork Sign(CachedWork work)
    {
        var image = Image.Load(work.Work, out var format);

        image.Metadata.ExifProfile.SetValue(ExifTag.Copyright, "COPYRIGHT REGISTERED BY CRPL");

        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, format);
        
        return new CachedWork
        {
            ContentType = work.ContentType,
            Work = memoryStream.ToArray()
        };
    }
}