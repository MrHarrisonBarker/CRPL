using CRPL.Data.Applications.DataModels;

namespace CRPL.Web.Services.Interfaces;

public interface IAccountManagementService
{
    public Task<DeleteAccountApplication> DeleteUser(DeleteAccountApplication deleteAccountApplication);
    public Task<WalletTransferApplication> WalletTransfer(WalletTransferApplication walletTransferApplication);
}