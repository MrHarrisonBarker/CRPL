using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Updaters;

public static class OwnershipRestructureUpdater
{
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