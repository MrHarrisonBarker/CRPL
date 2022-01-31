using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

public class VideoSigner : IWorkSigner
{
    public CachedWork Sign(CachedWork work)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(work.Work);
        
        var tagFile = TagLib.File.Create(new SimpleFileAbstraction(new SimpleFile(work.FileName, stream)));

        tagFile.Tag.Copyright = "COPYRIGHT REGISTERED BY CRPL";
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