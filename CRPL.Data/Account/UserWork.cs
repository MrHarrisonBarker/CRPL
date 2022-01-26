namespace CRPL.Data.Account;

public class UserWork
{
    public Guid UserId { get; set; }
    public UserAccount UserAccount { get; set; }
    public Guid WorkId { get; set; }
    public RegisteredWork RegisteredWork { get; set; }
}