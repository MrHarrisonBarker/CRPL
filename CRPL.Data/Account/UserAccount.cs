using CRPL.Data.Applications;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Account;

public class UserAccount
{
    public Guid Id { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DOB? DateOfBirth { get; set; }
    
    // country code
    public string? RegisteredJurisdiction { get; set; }
    
    public string? Email { get; set; }
    public string? DialCode { get; set; }
    public string? PhoneNumber { get; set; }

    [Owned]
    public class DOB
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public AccountStatus Status { get; set; } = AccountStatus.Created;

    public enum AccountStatus
    {
        Created,
        Incomplete,
        Complete
    }
    
    public UserWallet Wallet { get; set; }
    
    public List<UserWork>? UserWorks { get; set; }
    
    public string? AuthenticationToken { get; set; }
    
    public List<UserApplication>? Applications { get; set; }
}