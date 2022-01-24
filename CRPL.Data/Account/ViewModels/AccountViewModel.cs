using CRPL.Data.Account.StatusModels;

namespace CRPL.Data.Account.ViewModels;

public class UserAccountViewModel
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserAccount.DOB DateOfBirth { get; set; }
    
    // country code
    public string RegisteredJurisdiction { get; set; }
    
    // might not be needed
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public UserAccount.AccountStatus Status { get; set; }
}