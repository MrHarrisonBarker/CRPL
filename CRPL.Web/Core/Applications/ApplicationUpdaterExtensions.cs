using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.StructuredOwnership;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

// Extenstion methods for useful and repeated logic
public static class ApplicationUpdaterExtensions
{
    // Loop though all properties in the data model and input model
    // if any match update the data model with the input model
    public static Application UpdateProperties<T>(this Application application, T inputModel, List<string> ignored) where T : ApplicationInputModel
    {
        foreach (var property in typeof(T).GetProperties())
        {
            var destination = application.GetType().GetProperty(property.Name);
            if (destination != null && !ignored.Contains(property.Name))
            {
                var val = property.GetValue(inputModel);
                if (val != null) destination.SetValue(application, val);
            }
        }

        return application;
    }

    // Check all users exist then add them to the application
    public static Application CheckAndAssignStakes(this Application application, IUserService userService, List<OwnershipStake> stakes)
    {
        // removing duplicates (some of the current owners will probably still be in the new structure)
        var owners = stakes.Select(x => x.Owner.ToLower()).Distinct().ToList();

        if (!(userService.AreUsersReal(owners))) throw new Exception("Not all the users could be found");
        foreach (var owner in owners)
        {
            userService.AssignToApplication(owner, application.Id);
        }

        return application;
    }
}