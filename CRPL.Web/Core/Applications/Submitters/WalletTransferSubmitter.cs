using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

public static class WalletTransferSubmitter
{
    public static async Task<WalletTransferApplication> Submit(this WalletTransferApplication walletTransferApplication, IServiceProvider serviceProvider)
    {
        var accountManagementService = serviceProvider.GetRequiredService<IAccountManagementService>();
        
        walletTransferApplication.Status = ApplicationStatus.Submitted;
        
        return await accountManagementService.WalletTransfer(walletTransferApplication);
    }
}