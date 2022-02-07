using CRPL.Contracts.Standard.ContractDefinition;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public static class RegisteredEventProcessor
{
    public static Task ProcessEvent(this EventLog<RegisteredEventDTO> registeredEvent, IServiceProvider serviceProvider)
    {
        
        
        return Task.CompletedTask;
    }
}