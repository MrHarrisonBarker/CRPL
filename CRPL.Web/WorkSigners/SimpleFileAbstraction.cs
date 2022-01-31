namespace CRPL.Web.WorkSigners;

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