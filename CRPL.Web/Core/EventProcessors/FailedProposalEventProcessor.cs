using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Web.Core;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public static class FailedProposalEventProcessor
{
    public static async Task ProcessEvent(this EventLog<FailedProposalEventDTO> failedProposal, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing failed proposal event for {Id}", failedProposal.Event.RightId);
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        
        var application = await context.OwnershipRestructureApplications
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.AssociatedWork.RightId == failedProposal.Event.RightId.ToString());

        if (application == null) throw new ApplicationNotFoundException();
        
        context.Update(application);

        application.BindStatus = BindStatus.Rejected;
        application.Status = ApplicationStatus.Failed;

        await context.SaveChangesAsync();

        var resonanceService = scope.ServiceProvider.GetRequiredService<IResonanceService>();
        await resonanceService.PushApplicationUpdates(application);
    }
}