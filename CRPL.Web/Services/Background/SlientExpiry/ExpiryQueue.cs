using System.Collections.Concurrent;

namespace CRPL.Web.Services.Background.SlientExpiry;

public interface IExpiryQueue
{
    void QueueExpire(Guid id);
    Task<Guid> DequeueAsync(CancellationToken cancellationToken);
}

public class ExpiryQueue : IExpiryQueue
{
    private readonly ILogger<ExpiryQueue> Logger;
    private readonly ConcurrentQueue<Guid> Queue = new();
    private readonly SemaphoreSlim Signal = new(0);


    public ExpiryQueue(ILogger<ExpiryQueue> logger)
    {
        Logger = logger;
    }

    public void QueueExpire(Guid id)
    {
        if (id == null) throw new ArgumentNullException();
        Logger.LogInformation("queuing work for expiry {Id}", id);

        Queue.Enqueue(id);
        
        Signal.Release();
    }

    public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
    {
        await Signal.WaitAsync(cancellationToken);
        Queue.TryDequeue(out var eventLog);

        return eventLog;
    }
}