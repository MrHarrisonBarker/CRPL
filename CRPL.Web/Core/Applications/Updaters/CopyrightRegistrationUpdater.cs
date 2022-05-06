using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Updaters;

// An updater class for copyright registration applications
public static class CopyrightRegistrationUpdater
{
    private static readonly List<string> Encodables = new() { "OwnershipStakes", "Id"  };
    
    // When the user submits the application update the data model using input data,
    // encode ownership stakes into a string and assign any users added to the application
    public static async Task<CopyrightRegistrationApplication> Update(this CopyrightRegistrationApplication application, CopyrightRegistrationInputModel inputModel, IServiceProvider serviceProvider)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();

        // Encodables is a list of ignored properties on the model. They're either manually updated or not necessary.
        application.UpdateProperties(inputModel, Encodables);

        if (inputModel.OwnershipStakes != null)
        {
            application.OwnershipStakes = inputModel.OwnershipStakes.Encode();

            application.CheckAndAssignStakes(userService, inputModel.OwnershipStakes);
        }

        return application;
    }
}