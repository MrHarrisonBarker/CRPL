using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

// Digital signer for sound files by adding metadata to the file
public class SoundSigner : WorkSigner, IWorkSigner
{
    public SoundSigner(string signature) : base(signature)
    {
    }

    public CachedWork Sign(CachedWork work)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(work.Work);

        var file = new SimpleFile(work.FileName, stream);
        var abstractFile = new SimpleFileAbstraction(file);
        var tagFile = TagLib.File.Create(abstractFile);

        tagFile.Tag.Copyright = Signature;
        tagFile.Save();

        stream.Position = 0;
        using (var reader = new BinaryReader(stream))
        {
            return new CachedWork
            {
                ContentType = work.ContentType,
                FileName = work.FileName,
                Work = reader.ReadBytes((int)stream.Length)
            };
        }
    }
}