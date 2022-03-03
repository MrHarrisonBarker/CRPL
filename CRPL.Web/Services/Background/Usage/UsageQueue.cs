using System.Collections.Concurrent;
using CRPL.Data.Account.Works;

namespace CRPL.Web.Services.Background.Usage;

public interface IUsageQueue
{
    void QueueUsage(WorkUsage usage);
    Task<WorkUsage> DequeueAsync(CancellationToken cancellationToken);
}

public class UsageQueue : IUsageQueue
{
    private readonly ILogger<UsageQueue> Logger;
    private readonly ConcurrentQueue<WorkUsage> Queue = new();
    private readonly SemaphoreSlim Signal = new(0);

    public UsageQueue(ILogger<UsageQueue> logger)
    {
        Logger = logger;
    }
    
    public void QueueUsage(WorkUsage usage)
    {
        Logger.LogInformation("queuing work usage {WorkId}", usage.WorkId);

        Queue.Enqueue(usage);
        
        Signal.Release();
    }

    public async Task<WorkUsage> DequeueAsync(CancellationToken cancellationToken)
    {
        await Signal.WaitAsync(cancellationToken);
        Queue.TryDequeue(out var usage);

        return usage;
    }
}