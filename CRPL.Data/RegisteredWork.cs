namespace CRPL.Data.Account;

public class RegisteredWork
{
    public Guid Id { get; set; }
    public string RightId { get; set; }
    
    public UserWallet Wallet { get; set; }
}