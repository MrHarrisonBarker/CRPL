using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Updaters;

// An updater class for ownership restructure applications
public static class OwnershipRestructureUpdater
{
    // Attach the work to the restructure application, assign all users from the current and proposed ownership structures
    // save origin and restructure reason if they exist (when disputing)
    public static async Task<OwnershipRestructureApplication> Update(this OwnershipRestructureApplication application, OwnershipRestructureInputModel inputModel, IServiceProvider serviceProvider)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();
        var copyrightService = serviceProvider.GetRequiredService<ICopyrightService>();
        // TODO: Should check if the current structure is the correct structure

        if (inputModel.WorkId.HasValue) await copyrightService.AttachWorkToApplicationAndCheckValid(inputModel.WorkId.Value, application);

        if (inputModel.CurrentStructure.Count > 0 && inputModel.ProposedStructure.Count > 0)
        {
            application.CheckAndAssignStakes(userService, inputModel.CurrentStructure.Concat(inputModel.ProposedStructure).ToList());

            application.CurrentStructure = inputModel.CurrentStructure.Encode();
            application.ProposedStructure = inputModel.ProposedStructure.Encode();
        }

        application.Origin = inputModel.Origin;
        application.RestructureReason = inputModel.RestructureReason;

        return application;
    }
}