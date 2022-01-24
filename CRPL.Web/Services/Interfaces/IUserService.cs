using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;

namespace CRPL.Web.Services.Interfaces;

public interface IUserService
{
    // +----------------------+
    // +-------- Gets --------+
    // +----------------------+
    public Task<UserAccountStatusModel> GetAccount(Guid id);
    public Task<UserWallet> GetWallet(Guid accountId);

    // +----------------------+
    // -------- Updates ------+
    // +----------------------+
    public Task<UserAccountStatusModel> UpdateAccount(Guid accountId, AccountInputModel accountInputModel);
    public Task<string> UpdateWallet(Guid accountId, WalletInputModel walletInputModel);
    
    // +----------------------+
    // +--- Authentication ---+
    // +----------------------+
    public Task<long> FetchNonce(Guid accountId);
    public Task<long> FetchNonce(Guid accountId, string walletAddress);
    public Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel);
}