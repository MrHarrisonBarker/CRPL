using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;

namespace CRPL.Web.Services.Submitters;

// A Submitter class for dispute applications
public static class DisputeSubmitter
{
    // When the user submits the application update the status
    public static async Task<DisputeApplication> Submit(this DisputeApplication disputeApplication, IServiceProvider serviceProvider)
    {
        disputeApplication.Status = ApplicationStatus.Submitted;
        
        return disputeApplication;
    }
}