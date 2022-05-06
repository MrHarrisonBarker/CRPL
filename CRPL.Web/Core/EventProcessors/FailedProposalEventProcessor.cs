using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Web.Core;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

// Blockchain event processor for the Failed Proposal event
public static class FailedProposalEventProcessor
{
    // When a proposal fails update application status to Failed
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

        // Send application updates to websocket subscribers
        var resonanceService = scope.ServiceProvider.GetRequiredService<IResonanceService>();
        await resonanceService.PushApplicationUpdates(application);
    }
}