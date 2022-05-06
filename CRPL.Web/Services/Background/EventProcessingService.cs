using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Web.Services.Background.EventProcessors;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background;

// A background service for processing blockchain events
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

    // Dequeue event and process
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting event processing queue");

        while (!cancellationToken.IsCancellationRequested)
        {
            var nextEvent = await EventQueue.DequeueAsync(cancellationToken);

            try
            {
                Logger.LogInformation("Processing next event");

                // switch for casting to the correct event type
                switch (nextEvent.GetType().FullName)
                {
                    case var name when name.Contains("RegisteredEvent"):
                        await ((EventLog<RegisteredEventDTO>)nextEvent).ProcessEvent(ServiceProvider, Logger);
                        break;
                    case var name when name.Contains("ApprovedEvent"):
                        await ((EventLog<ApprovedEventDTO>)nextEvent).ProcessEvent(ServiceProvider, Logger);
                        break;
                    case var name when name.Contains("ProposedRestructureEvent"):
                        await ((EventLog<ProposedRestructureEventDTO>)nextEvent).ProcessEvent(ServiceProvider, Logger);
                        break;
                    case var name when name.Contains("RestructuredEvent"):
                        await ((EventLog<RestructuredEventDTO>)nextEvent).ProcessEvent(ServiceProvider, Logger);
                        break;
                    case var name when name.Contains("FailedProposalEvent"):
                        await ((EventLog<FailedProposalEventDTO>)nextEvent).ProcessEvent(ServiceProvider, Logger);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred when processing event {Event}", nextEvent.GetType().FullName);
            }
        }
    }
}