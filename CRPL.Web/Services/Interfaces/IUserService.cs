using CRPL.Data.Account.InputModels;
using CRPL.Data.Account.StatusModels;
using CRPL.Data.Account.ViewModels;

namespace CRPL.Web.Services.Interfaces;

public interface IUserService
{
    // +----------------------+
    // +-------- Gets --------+
    // +----------------------+
    public Task<UserAccountStatusModel> GetAccount(Guid id);
    public Task<bool> IsUniquePhoneNumber(Guid id, string phoneNumber);
    public Task<bool> IsUniqueEmail(Guid id, string email);
    public bool AreUsersReal(List<string> userAddresses);
    public Task<List<UserAccountMinimalViewModel>> SearchUsers(string address);

    // +----------------------+
    // -------- Updates ------+
    // +----------------------+
    public Task<UserAccountStatusModel> UpdateAccount(Guid accountId, AccountInputModel accountInputModel);
    public void AssignToApplication(string address, Guid applicationId);
    public void AssignToApplication(Guid id, Guid applicationId);

    // +----------------------+
    // +--- Authentication ---+
    // +----------------------+
    // +----------------------+
    public Task<string> FetchNonce(string walletId);
    public Task<AuthenticateResult> AuthenticateSignature(AuthenticateSignatureInputModel authenticateInputModel);
    public Task<UserAccountViewModel> Authenticate(string token);
    public Task RevokeAuthentication(string token);
    public Task<bool> IsShareholder(string address, string rightId);
}