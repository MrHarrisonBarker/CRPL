using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public abstract class EventProcessor
{
    public abstract Task ProcessEvent<T>(T eventLog, IServiceProvider serviceProvider) where T : IEventLog;
}