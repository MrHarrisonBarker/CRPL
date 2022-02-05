using AutoMapper;
using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public static class ApplicationUpdater
{
    private static readonly List<string> Encodables = new() { "OwnershipStakes" };
    
    public static Application Update(this Application application, ApplicationInputModel inputModel, IMapper mapper, IUserService userService)
    {
        switch (application.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return CopyrightRegistrationUpdater((CopyrightRegistrationApplication)application, (CopyrightRegistrationInputModel)inputModel, userService);
            case ApplicationType.OwnershipRestructure:
                return OwnershipRestructureUpdater((OwnershipRestructureApplication)application, (OwnershipRestructureInputModel)inputModel, userService);
            case ApplicationType.CopyrightTypeChange:
                break;
            case ApplicationType.Dispute:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return application;
    }

    private static Application OwnershipRestructureUpdater(OwnershipRestructureApplication application, OwnershipRestructureInputModel inputModel, IUserService userService)
    {
        // TODO: Should check if the current structure is the correct structure
        
        if (inputModel.CurrentStructure.Count > 0 && inputModel.ProposedStructure.Count > 0)
        {
            application.CheckAndAssignStakes(userService, inputModel.CurrentStructure.Concat(inputModel.ProposedStructure).ToList());
            
            application.CurrentStructure = inputModel.CurrentStructure.Encode();
            application.ProposedStructure = inputModel.ProposedStructure.Encode();
        }

        return application;
    }

    private static Application CopyrightRegistrationUpdater(CopyrightRegistrationApplication application, CopyrightRegistrationInputModel inputModel, IUserService userService)
    {
        application.UpdateProperties(inputModel, Encodables.Concat(new List<string> { "Id" }).ToList());

        if (inputModel.OwnershipStakes != null)
        {
            application.OwnershipStakes = inputModel.OwnershipStakes.Encode();

            application.CheckAndAssignStakes(userService, inputModel.OwnershipStakes);
        }

        return application;
    }

    private static Application UpdateProperties<T>(this Application application, T inputModel, List<string> ignored) where T : ApplicationInputModel
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
    
    private static Application CheckAndAssignStakes(this Application application, IUserService userService, List<OwnershipStake> stakes)
    {
        var owners = stakes.Select(x => x.Owner).Distinct().ToList();

        if (!(userService.AreUsersReal(owners))) throw new Exception("Not all the users could be found");
        foreach (var owner in owners)
        {
            userService.AssignToApplication(owner, application.Id);
        }
        
        return application;
    }
}