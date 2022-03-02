using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;

namespace CRPL.Web.Services.Submitters;

public static class DisputeSubmitter
{
    public static async Task<DisputeApplication> Submit(this DisputeApplication disputeApplication, IServiceProvider serviceProvider)
    {
        disputeApplication.Status = ApplicationStatus.Submitted;
        
        return disputeApplication;
    }
}