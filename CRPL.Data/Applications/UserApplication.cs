using CRPL.Data.Account;

namespace CRPL.Data.Applications;

public class UserApplication
{
    public Guid ApplicationId { get; set; }
    public Application Application { get; set; }
    public Guid UserId { get; set; }
    public UserAccount UserAccount { get; set; }
}