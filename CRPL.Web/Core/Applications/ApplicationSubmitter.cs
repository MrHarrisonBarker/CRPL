using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Submitters;

namespace CRPL.Web.Services;

// Class used for casting incoming application submits to the correct submitter
public static class ApplicationSubmitter
{
    public static async Task<Application> SubmitApplication(this Application submittedApplication, IServiceProvider serviceProvider)
    {
        submittedApplication.Modified = DateTime.Now;
        
        switch (submittedApplication.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return await ((CopyrightRegistrationApplication)submittedApplication).Submit(serviceProvider);
            case ApplicationType.OwnershipRestructure:
                return await ((OwnershipRestructureApplication)submittedApplication).Submit(serviceProvider);
            case ApplicationType.Dispute:
                return await ((DisputeApplication)submittedApplication).Submit(serviceProvider);
            case ApplicationType.DeleteAccount:
                return await ((DeleteAccountApplication)submittedApplication).Submit(serviceProvider);
            case ApplicationType.WalletTransfer:
                return await ((WalletTransferApplication)submittedApplication).Submit(serviceProvider);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}