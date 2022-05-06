using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

// A Submitter class for delete account applications
public static class DeleteAccountSubmitter
{
    // When the user submits the application update the status and delete user
    public static async Task<DeleteAccountApplication> Submit(this DeleteAccountApplication deleteAccountApplication, IServiceProvider serviceProvider)
    {
        var accountManagementService = serviceProvider.GetRequiredService<IAccountManagementService>();
        
        deleteAccountApplication.Status = ApplicationStatus.Submitted;

        return await accountManagementService.DeleteUser(deleteAccountApplication);
    }
}