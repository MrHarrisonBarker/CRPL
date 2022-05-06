using System.Collections.Concurrent;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background;

public interface IEventQueue
{
    void QueueEvent(IEventLog log);
    Task<IEventLog> DequeueAsync(CancellationToken cancellationToken);
}

// Queue holding blockchain events
public class EventQueue : IEventQueue
{
    private readonly ILogger<EventQueue> Logger;
    private readonly ConcurrentQueue<IEventLog> Queue = new();
    private readonly SemaphoreSlim Signal = new(0);


    public EventQueue(ILogger<EventQueue> logger)
    {
        Logger = logger;
    }

    public void QueueEvent(IEventLog log)
    {
        if (log == null) throw new ArgumentNullException();
        Logger.LogInformation("queuing event {Id}", log.Log.TransactionHash);

        Queue.Enqueue(log);
        
        Signal.Release();
    }

    public async Task<IEventLog> DequeueAsync(CancellationToken cancellationToken)
    {
        await Signal.WaitAsync(cancellationToken);
        Queue.TryDequeue(out var eventLog);

        return eventLog;
    }
}