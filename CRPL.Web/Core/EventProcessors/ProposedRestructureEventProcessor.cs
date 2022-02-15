using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public static class ProposedRestructureEventProcessor
{
    public static async Task ProcessEvent(this EventLog<ProposedRestructureEventDTO> proposedRestructure, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing proposed event for {Id}", proposedRestructure.Event.RightId);
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        
        var application = await context.OwnershipRestructureApplications.Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.AssociatedWork.RightId == proposedRestructure.Event.RightId.ToString());

        if (application == null) throw new ApplicationNotFoundException();
        
        context.Update(application);

        application.BindStatus = BindStatus.AwaitingVotes;

        await context.SaveChangesAsync();
    }
}