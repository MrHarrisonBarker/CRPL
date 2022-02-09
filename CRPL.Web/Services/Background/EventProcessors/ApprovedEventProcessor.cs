using CRPL.Contracts.Standard.ContractDefinition;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public static class ApprovedEventProcessor
{
    public static async Task ProcessEvent(this EventLog<ApprovedEventDTO> approvedEvent, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing approved event for {Id}", approvedEvent.Event.RightId);
    }
}