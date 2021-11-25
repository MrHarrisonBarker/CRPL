namespace PIPO.Verify;

public struct VerificationResult
{
    public bool isOriginal;
}

public interface IWorksVerificationService
{
    public Task<VerificationResult> VerifyWork(byte[] work);
}

public class WorksVerificationService : IWorksVerificationService
{
    private readonly List<byte[]> RegisteredWorks;

    public WorksVerificationService()
    {
        RegisteredWorks = new List<byte[]>();
        RegisterWorks();
    }

    private async void RegisterWorks()
    {
        RegisteredWorks.Add(Utils.LoadWorkIntoByteArray("./works/work1.jpg"));
        RegisteredWorks.Add(Utils.LoadWorkIntoByteArray("./works/work2.jpg"));
    }

    public async Task<VerificationResult> VerifyWork(byte[] work)
    {
        try
        {
            // Generic work verification pipeline
            // 1. Check hash of work against other hashes (will be pre computed in db but for now its computed on the fly from local files)

            var original = true;

            foreach (var registeredWork in RegisteredWorks)
            {
                if (VerifyHelpers.CompareWork(registeredWork, work)) original = false ;
            }

            return new VerificationResult()
            {
                isOriginal = original
            };
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}