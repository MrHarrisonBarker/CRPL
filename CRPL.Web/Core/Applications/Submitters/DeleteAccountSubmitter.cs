using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

public static class DeleteAccountSubmitter
{
    public static async Task<DeleteAccountApplication> Submit(this DeleteAccountApplication deleteAccountApplication, IServiceProvider serviceProvider)
    {
        var accountManagementService = serviceProvider.GetRequiredService<IAccountManagementService>();
        
        deleteAccountApplication.Status = ApplicationStatus.Submitted;

        return await accountManagementService.DeleteUser(deleteAccountApplication);
    }
}