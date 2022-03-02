using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.StructuredOwnership;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public static class ApplicationUpdaterExtensions
{
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

    public static Application CheckAndAssignStakes(this Application application, IUserService userService, List<OwnershipStake> stakes)
    {
        var owners = stakes.Select(x => x.Owner.ToLower()).Distinct().ToList();

        if (!(userService.AreUsersReal(owners))) throw new Exception("Not all the users could be found");
        foreach (var owner in owners)
        {
            userService.AssignToApplication(owner, application.Id);
        }

        return application;
    }
}