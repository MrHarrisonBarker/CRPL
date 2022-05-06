using CRPL.Data.Works;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace CRPL.Web.WorkSigners;

// Digital signer for image files by adding EXIF data
public class ImageSigner : WorkSigner, IWorkSigner
{
    public ImageSigner(string signature) : base(signature)
    {
    }
    
    public CachedWork Sign(CachedWork work)
    {
        var image = Image.Load(work.Work, out var format);

        if (image != null)
        {
            if (image.Metadata.ExifProfile == null) image.Metadata.ExifProfile = new ExifProfile();
            image.Metadata.ExifProfile.SetValue(ExifTag.Copyright, Signature);
        }

        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, format);

        return new CachedWork
        {
            ContentType = work.ContentType,
            Work = memoryStream.ToArray()
        };
    }
}