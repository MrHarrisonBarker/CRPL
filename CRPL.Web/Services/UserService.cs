using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

// all methods effecting user accounts and user wallets
public class UserService : IUserService
{
    public Task<UserAccountStatusModel> GetAccount(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<UserWallet> GetWallet(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public Task<UserAccountStatusModel> UpdateAccount(Guid accountId, AccountInputModel accountInputModel)
    {
        throw new NotImplementedException();
    }

    public Task<string> UpdateWallet(Guid accountId, WalletInputModel walletInputModel)
    {
        throw new NotImplementedException();
    }

    public Task<long> FetchNonce(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public Task<long> FetchNonce(Guid accountId, string walletAddress)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel)
    {
        throw new NotImplementedException();
    }
}