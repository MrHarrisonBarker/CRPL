using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Updaters;

// An updater class for dispute applications
public static class DisputeUpdater
{
    private static readonly List<string> Encodables = new() { "OwnershipStakes" };
    
    // Update data model properties, assign the accuser to the application, attach the disputed work to the dispute application
    public static async Task<DisputeApplication> Update(this DisputeApplication application, DisputeInputModel inputModel, IServiceProvider serviceProvider)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();
        var copyrightService = serviceProvider.GetRequiredService<ICopyrightService>();
        
        // Encodables are ignored properties
        application.UpdateProperties(inputModel, Encodables.Concat(new List<string> { "Id", "DisputedWork", "Accuser", "ResolveResult" }).ToList());

        if (inputModel.AccuserId.HasValue)
        {
            userService.AssignToApplication(inputModel.AccuserId.Value, application.Id);
        }

        if (inputModel.DisputedWorkId.HasValue)
        {
            await copyrightService.AttachWorkToApplicationAndCheckValid(inputModel.DisputedWorkId.Value, application);
        }

        return application;
    }
}