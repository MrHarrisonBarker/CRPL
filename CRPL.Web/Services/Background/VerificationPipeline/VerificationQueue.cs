using System.Collections.Concurrent;

namespace CRPL.Web.Services.Background.VerificationPipeline;

public interface IVerificationQueue
{
    void QueueWork(Guid id);
    Task<Guid> DequeueAsync(CancellationToken cancellationToken);
}

public class VerificationQueue : IVerificationQueue
{
    private readonly ILogger<VerificationQueue> Logger;
    private readonly ConcurrentQueue<Guid> Queue = new();
    private readonly SemaphoreSlim Signal = new(0);

    public VerificationQueue(ILogger<VerificationQueue> logger)
    {
        Logger = logger;
    }
    
    public void QueueWork(Guid id)
    {
        if (id == null || id == Guid.Empty) throw new ArgumentNullException();
        Logger.LogInformation("queuing work to be verified {Id}", id);

        Queue.Enqueue(id);
        
        Signal.Release();
    }

    public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        await Signal.WaitAsync(cancellationToken);
        Queue.TryDequeue(out var workId);

        await Task.Delay((int)TimeSpan.FromSeconds(10).TotalMilliseconds);

        return workId;
    }
}