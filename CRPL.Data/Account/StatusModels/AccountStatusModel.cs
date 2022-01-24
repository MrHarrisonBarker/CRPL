using CRPL.Data.Account.ViewModels;

namespace CRPL.Data.Account.StatusModels;

// returns when updating an account describing whats left
public class UserAccountStatusModel
{
    public UserAccountViewModel UserAccount { get; set; }
    public List<PartialField>? PartialFields { get; set; }
}