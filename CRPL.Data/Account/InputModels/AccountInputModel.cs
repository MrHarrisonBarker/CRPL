namespace CRPL.Data.Account.InputModels;

public class AccountInputModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserAccount.DOB? DateOfBirth { get; set; }
    
    // country code
    public string? RegisteredJurisdiction { get; set; }
    public string? Email { get; set; }
    public string? DialCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FcmToken { get; set; }
}