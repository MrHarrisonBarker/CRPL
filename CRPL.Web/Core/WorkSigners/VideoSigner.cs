using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

// Digital singer for video files by adding metadata to the file
public class VideoSigner : WorkSigner, IWorkSigner
{
    public VideoSigner(string signature) : base(signature)
    {
    }

    public CachedWork Sign(CachedWork work)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(work.Work);

        var tagFile = TagLib.File.Create(new SimpleFileAbstraction(new SimpleFile(work.FileName, stream)));

        tagFile.Tag.Copyright = Signature;
        tagFile.Save();

        stream.Position = 0;
        using var reader = new BinaryReader(stream);
        return new CachedWork
        {
            ContentType = work.ContentType,
            FileName = work.FileName,
            Work = reader.ReadBytes((int)stream.Length)
        };
    }
}