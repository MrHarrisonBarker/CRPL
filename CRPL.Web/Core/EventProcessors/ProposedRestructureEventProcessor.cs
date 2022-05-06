using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Proposal;
using CRPL.Web.Core;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

// Blockchain event processor for the Proposed Restructure event
public static class ProposedRestructureEventProcessor
{
    // When a restructure is proposed update status
    public static async Task ProcessEvent(this EventLog<ProposedRestructureEventDTO> proposedRestructure, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing proposed event for {Id}", proposedRestructure.Event.RightId);
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        
        var application = await context.OwnershipRestructureApplications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers)
            .Include(x => x.Origin)
            .Where(x => x.Status == ApplicationStatus.Submitted && x.BindStatus == BindStatus.NoProposal)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(x => x.AssociatedWork.RightId == proposedRestructure.Event.RightId.ToString());

        if (application == null) throw new ApplicationNotFoundException();
        
        context.Update(application);

        // If the restructure is a result of a delete account application then automatically bind the proposal
        if (application.RestructureReason == RestructureReason.DeleteAccount)
        {
            var copyrightService = scope.ServiceProvider.GetRequiredService<ICopyrightService>();
            await copyrightService.BindProposal(new BindProposalInput
            {
                Accepted = true,
                ApplicationId = ((DeleteAccountApplication)application.Origin).AccountId
            });
        }

        application.BindStatus = BindStatus.AwaitingVotes;

        await context.SaveChangesAsync();
        
        // Send application updates to websocket subscribers
        var resonanceService = scope.ServiceProvider.GetRequiredService<IResonanceService>();
        await resonanceService.PushApplicationUpdates(application);
    }
}