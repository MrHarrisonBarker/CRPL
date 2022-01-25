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
    // +----------------------+
    public Task<string> FetchNonce(string walletId);
    public Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel);
    public Task Authenticate(string token);
    public Task RevokeAuthentication(string token);
}