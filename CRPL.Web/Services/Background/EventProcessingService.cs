using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Web.Services.Background.EventProcessors;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background;

public class EventProcessingService : BackgroundService
{
    private readonly ILogger<EventProcessingService> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly IEventQueue EventQueue;

    public EventProcessingService(ILogger<EventProcessingService> logger, IServiceProvider serviceProvider, IEventQueue eventQueue)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        EventQueue = eventQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting event processing queue");

        while (!cancellationToken.IsCancellationRequested)
        {
            var nextEvent = await EventQueue.DequeueAsync(cancellationToken);

            try
            {
                Logger.LogInformation("Processing next event");

                switch (nextEvent.GetType().FullName)
                {
                    case var name when name.Contains(""):
                        await ((EventLog<RegisteredEventDTO>)nextEvent).ProcessEvent(ServiceProvider);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred when processing event {Event}", nameof(nextEvent));
            }
        }
    }
}