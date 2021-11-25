namespace PIPO.Verify;

public static class Utils
{
    public static byte[] LoadWorkIntoByteArray(string path)
    {
        try
        {
            var stream = File.OpenRead(path);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer);

            return buffer;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}