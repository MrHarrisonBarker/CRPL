using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;

namespace CRPL.Web.Services.Interfaces;

public interface IAccountManagementService
{
    public Task<Application> DeleteUser(DeleteAccountApplication deleteAccountApplication);
    public Task<Application> WalletTransfer(WalletTransferApplication walletTransferApplication);
}