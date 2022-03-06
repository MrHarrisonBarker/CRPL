using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Web.Core;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services.Background.EventProcessors;

public static class RestructuredEventProcessor
{
    public static async Task ProcessEvent(this EventLog<RestructuredEventDTO> restructuredEvent, IServiceProvider serviceProvider, ILogger<EventProcessingService> logger)
    {
        logger.LogInformation("Processing restructured event for {Id}", restructuredEvent.Event.RightId);

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        var work = await context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .FirstOrDefaultAsync(x => x.RightId == restructuredEvent.Event.RightId.ToString());

        if (work == null) throw new WorkNotFoundException();
        if (work.Status != RegisteredWorkStatus.Registered) throw new WorkNotRegisteredException();

        context.Update(work);

        // assigning new shareholders to the work and removing old ones
        logger.LogInformation("Assigning new shareholders and removing old");
        
        context.UserWorks.RemoveRange(work.UserWorks);
        work.UserWorks.Clear();
        
        foreach (var ownershipStake in restructuredEvent.Event.Proposal.NewStructure)
        {
            await AssignWorkToUser(logger, context, ownershipStake, work);
        }

        logger.LogInformation("Setting restructure application to complete");
        
        await SetApplicationStatus(work, context);

        await context.SaveChangesAsync();
        
        var resonanceService = scope.ServiceProvider.GetRequiredService<IResonanceService>();
        await resonanceService.PushWorkUpdates(work);
    }

    private static async Task SetApplicationStatus(RegisteredWork work, ApplicationContext context)
    {
        var application = (OwnershipRestructureApplication)work.AssociatedApplication.FirstOrDefault(x => x.Status == ApplicationStatus.Submitted && x.ApplicationType == ApplicationType.OwnershipRestructure);

        if (application == null) throw new ApplicationNotFoundException();
        
        // getting the application again from the database, this was done to also get the origin
        application = await context.OwnershipRestructureApplications
            .Include(x => x.Origin)
            .FirstOrDefaultAsync(x => x.Id == application.Id);
        
        if (application == null) throw new ApplicationNotFoundException();

        application.Status = ApplicationStatus.Complete;
        application.BindStatus = BindStatus.Bound;

        // if the restructure is a result of another application set that application now to complete
        if (application.Origin != null) application.Origin.Status = ApplicationStatus.Complete;
    }

    private static async Task AssignWorkToUser(ILogger<EventProcessingService> logger, ApplicationContext context, OwnershipStakeContract x, RegisteredWork work)
    {
        var user = await context.UserAccounts.FirstOrDefaultAsync(u => u.Wallet.PublicAddress.ToLower() == x.Owner.ToLower());

        if (user == null) throw new UserNotFoundException(x.Owner);

        logger.LogInformation("Assigning {Address} to work {Id}", x.Owner, work.RightId);

        context.UserWorks.Add(new UserWork()
        {
            UserId = user.Id,
            WorkId = work.Id
        });
    }
}