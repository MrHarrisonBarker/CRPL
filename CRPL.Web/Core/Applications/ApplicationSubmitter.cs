using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public static class ApplicationSubmitter
{
    public static async Task<Application> Submit(this Application submittedApplication, IRegistrationService registrationService, ICopyrightService copyrightService)
    {
        switch (submittedApplication.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return CopyrightRegistrationSubmitter((CopyrightRegistrationApplication)submittedApplication, registrationService);
            case ApplicationType.OwnershipRestructure:
                return await OwnershipRestructureSubmitter((OwnershipRestructureApplication)submittedApplication, copyrightService);
            case ApplicationType.CopyrightTypeChange:
                break;
            case ApplicationType.Dispute:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return submittedApplication;
    }

    private static Application CopyrightRegistrationSubmitter(CopyrightRegistrationApplication copyrightRegistrationApplication, IRegistrationService registrationService)
    {
        registrationService.StartRegistration(copyrightRegistrationApplication);

        copyrightRegistrationApplication.Status = ApplicationStatus.Submitted;
        
        return copyrightRegistrationApplication;
    }
    
    private static async Task<Application> OwnershipRestructureSubmitter(OwnershipRestructureApplication ownershipRestructureApplication, ICopyrightService copyrightService)
    {
        ownershipRestructureApplication = await copyrightService.ProposeRestructure(ownershipRestructureApplication);

        ownershipRestructureApplication.Status = ApplicationStatus.Submitted;
        
        return ownershipRestructureApplication;
    }
}