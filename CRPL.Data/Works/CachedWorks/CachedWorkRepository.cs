using Microsoft.Extensions.Logging;

namespace CRPL.Data.Works;

public interface ICachedWorkRepository
{
    public CachedWork Get(byte[] hash);
    public void Set(byte[] hash, byte[] work, string contentType, string name);
}

public class CachedWorkRepository : ICachedWorkRepository
{
    private readonly ILogger<CachedWorkRepository> Logger;
    private Dictionary<string, CachedWork> CachedWorks;
    private Dictionary<string, CachedWork> CachedSignedWorks;

    public CachedWorkRepository(ILogger<CachedWorkRepository> logger)
    {
        Logger = logger;
        CachedWorks = new Dictionary<string, CachedWork>();
        CachedSignedWorks = new Dictionary<string, CachedWork>();
    }

    public CachedWork Get(byte[] hash)
    {
        Logger.LogInformation("Getting a cached work {Hash}", hash);
        if (!CachedWorks.TryGetValue(Convert.ToBase64String(hash), out var work)) throw new Exception($"Cannot get cached work, {Convert.ToBase64String(hash)}");
        CachedWorks.Remove(Convert.ToBase64String(hash));
        return work;
    }

    public void Set(byte[] hash, byte[] work, string contentType, string name)
    {
        Logger.LogInformation("caching work {Hash}", hash);
        CachedWorks.Add(Convert.ToBase64String(hash), new CachedWork()
        {
            Work = work,
            ContentType = contentType,
            FileName = name
        });
    }
}