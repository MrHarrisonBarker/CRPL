using CRPL.Contracts.Copyright.ContractDefinition;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

// Blockchain event processor for the Approved event
public static class ApprovedEventProcessor
{
    public static async Task ProcessEvent(this EventLog<ApprovedEventDTO> approvedEvent, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing approved event for {Id}", approvedEvent.Event.RightId);
    }
}