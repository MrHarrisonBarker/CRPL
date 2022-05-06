using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Web.Core;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

// Blockchain event processor for the Registered event
public static class RegisteredEventProcessor
{
    // When a work is registered update status and digitally sign the work
    public static async Task ProcessEvent(this EventLog<RegisteredEventDTO> registeredEvent, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing registered event for {Id}", registeredEvent.Event.RightId);

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var registeredWork = await context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .FirstOrDefaultAsync(x => x.RegisteredTransactionId == registeredEvent.Log.TransactionHash);
        if (registeredWork == null) throw new WorkNotFoundException(registeredEvent.Log.TransactionHash);

        context.Update(registeredWork);

        // Set statuses
        
        registeredWork.RightId = registeredEvent.Event.RightId.ToString();
        registeredWork.Registered = DateTime.Now;
        registeredWork.Status = RegisteredWorkStatus.Registered;

        registeredWork.AssociatedApplication.First(x => x.ApplicationType == ApplicationType.CopyrightRegistration).Status = ApplicationStatus.Complete;
        
        // Digitally signing the registered work
        var worksVerificationService = scope.ServiceProvider.GetRequiredService<IWorksVerificationService>();
        await worksVerificationService.Sign(registeredWork);
        
        await context.SaveChangesAsync();
        
        // Send application and work updates to websocket subscribers
        var resonanceService = scope.ServiceProvider.GetRequiredService<IResonanceService>();
        await resonanceService.PushWorkUpdates(registeredWork);
        await resonanceService.PushApplicationUpdates(registeredWork.AssociatedApplication.First(x => x.ApplicationType == ApplicationType.CopyrightRegistration));
    }
}