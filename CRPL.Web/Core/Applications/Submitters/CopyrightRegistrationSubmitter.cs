using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

// A Submitter class for copyright registration applications
public static class CopyrightRegistrationSubmitter
{
    // When the application is submitted it starts the registration process in the registration service
    // and updates the status
    public static async Task<CopyrightRegistrationApplication> Submit(this CopyrightRegistrationApplication copyrightRegistrationApplication, IServiceProvider serviceProvider)
    {
        var registrationService = serviceProvider.GetRequiredService<IRegistrationService>();

        await registrationService.StartRegistration(copyrightRegistrationApplication);

        copyrightRegistrationApplication.Status = ApplicationStatus.Submitted;

        return copyrightRegistrationApplication;
    }
}