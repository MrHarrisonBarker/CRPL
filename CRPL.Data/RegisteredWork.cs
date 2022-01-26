namespace CRPL.Data.Account;

public class RegisteredWork
{
    public Guid Id { get; set; }
    
    // maps from a bigInt aka uin256
    public string RightId { get; set; }
    
    public List<UserWork> UserWorks { get; set; }
}