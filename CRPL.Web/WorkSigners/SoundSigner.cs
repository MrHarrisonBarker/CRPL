using ATL;
using CRPL.Data.Works;

namespace CRPL.Web.WorkSigners;

public class SoundSigner : IWorkSigner
{
    public CachedWork Sign(CachedWork work)
    {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);
        writer.Write(work.Work);
        
        var file = new SimpleFile(work.FileName, stream);
        var abstractFile = new SimpleFileAbstraction(file);
        var tagFile = TagLib.File.Create(abstractFile);

        tagFile.Tag.Copyright = "COPYRIGHT REGISTERED BY CRPL";
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

public class SimpleFile
{
    public SimpleFile(string Name, Stream Stream)
    {
        this.Name = Name;
        this.Stream = Stream;
    }

    public string Name { get; set; }
    public Stream Stream { get; set; }
}

public class SimpleFileAbstraction : TagLib.File.IFileAbstraction
{
    private SimpleFile file;

    public SimpleFileAbstraction(SimpleFile file)
    {
        this.file = file;
    }

    public string Name
    {
        get { return file.Name; }
    }

    public System.IO.Stream ReadStream
    {
        get { return file.Stream; }
    }

    public System.IO.Stream WriteStream
    {
        get { return file.Stream; }
    }

    public void CloseStream(System.IO.Stream stream)
    {
        stream.Position = 0;
    }
}