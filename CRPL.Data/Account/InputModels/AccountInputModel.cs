namespace CRPL.Data.Account.InputModels;

public class AccountInputModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserAccount.DOB? DateOfBirth { get; set; }
    
    // country code
    public string? RegisteredJurisdiction { get; set; }
    
    // might not be needed
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}