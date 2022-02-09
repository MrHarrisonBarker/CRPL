using CRPL.Contracts.Standard.ContractDefinition;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public static class ProposedRestructureEventProcessor
{
    public static async Task ProcessEvent(this EventLog<ProposedRestructureEventDTO> proposedRestructure, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing proposed event for {Id}", proposedRestructure.Event.RightId);
    }
}