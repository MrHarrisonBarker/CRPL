using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

public static  class CopyrightRegistrationSubmitter
{
    public static async Task<CopyrightRegistrationApplication> Submit(this CopyrightRegistrationApplication copyrightRegistrationApplication, IServiceProvider serviceProvider)
    {
        var registrationService = serviceProvider.GetRequiredService<IRegistrationService>();
        
        await registrationService.StartRegistration(copyrightRegistrationApplication);

        copyrightRegistrationApplication.Status = ApplicationStatus.Submitted;
        
        return copyrightRegistrationApplication;
    }
}