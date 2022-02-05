using AutoMapper;
using CRPL.Data.Applications;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public static class ApplicationUpdater
{
    public static Application Update(this Application application, ApplicationInputModel inputModel, IMapper mapper, IUserService userService)
    {
        switch (application.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return CopyrightRegistrationUpdater((CopyrightRegistrationApplication)application, (CopyrightRegistrationInputModel)inputModel, userService);
            case ApplicationType.OwnershipRestructure:
                break;
            case ApplicationType.CopyrightTypeChange:
                break;
            case ApplicationType.Dispute:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return application;
    }

    private static readonly List<string> Encodables = new() { "OwnershipStakes" };

    private static Application CopyrightRegistrationUpdater(CopyrightRegistrationApplication application, CopyrightRegistrationInputModel inputModel, IUserService userService)
    {
        foreach (var property in typeof(CopyrightRegistrationInputModel).GetProperties())
        {
            var destination = typeof(CopyrightRegistrationApplication).GetProperty(property.Name);
            if (destination != null && !Encodables.Contains(property.Name) && property.Name != "Id")
            {
                var val = property.GetValue(inputModel);
                if (val != null) destination.SetValue(application, val);
            }
        }

        if (inputModel.OwnershipStakes != null)
        {
            application.OwnershipStakes = inputModel.OwnershipStakes.Encode();

            if (!(userService.AreUsersReal(inputModel.OwnershipStakes.Select(x => x.Owner).ToList()))) throw new Exception("Not all the users could be found");
            
            // assigning associated users

            foreach (var stake in inputModel.OwnershipStakes)
            {
                userService.AssignToApplication(stake.Owner, application.Id);
            }
        }
        

        return application;
    }
}