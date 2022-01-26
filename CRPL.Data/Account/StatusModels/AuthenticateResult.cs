using CRPL.Data.Account.ViewModels;

namespace CRPL.Data.Account.StatusModels;

// the final result of the authenticate pipeline = jwt
public class AuthenticateResult
{
    public string? Token { get; set; }
    public UserAccountViewModel? Account { get; set; }
    public string? Log { get; set; }
}