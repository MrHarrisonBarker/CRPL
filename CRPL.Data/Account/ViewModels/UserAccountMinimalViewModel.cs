namespace CRPL.Data.Account.ViewModels;

public class UserAccountMinimalViewModel
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string WalletPublicAddress { get; set; }
    public string? WalletAddressUri { get; set; }
}