using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
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
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var work = await context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .FirstOrDefaultAsync(x => x.RightId == restructuredEvent.Event.RightId.ToString());

        if (work == null) throw new WorkNotFoundException();

        context.Update(work);

        logger.LogInformation("Assigning new shareholders and removing old");
        // assigning new shareholders to the work and removing old ones
        work.UserWorks.Clear();
        restructuredEvent.Event.Proposal.NewStructure.ForEach(x => userService.AssignToWork(x.Owner, work));

        logger.LogInformation("Setting restructure application to complete");
        // setting application status to complete
        var application = work.AssociatedApplication.FirstOrDefault(x => x.Status == ApplicationStatus.Submitted && x.ApplicationType == ApplicationType.OwnershipRestructure);
        if (application == null) throw new ApplicationNotFoundException();
        application.Status = ApplicationStatus.Complete;

        await context.SaveChangesAsync();
    }
}