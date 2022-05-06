using CRPL.Data.Account;

namespace CRPL.Data.Applications;

public abstract class Application
{
    public Guid Id { get; set; }
    public ApplicationType ApplicationType { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public ApplicationStatus Status { get; set; }
    
    // Many to many relationship, User <-> Application
    public List<UserApplication> AssociatedUsers { get; set; }
    
    // One to many relationship, Application <- Work
    // works have many application
    public RegisteredWork? AssociatedWork { get; set; }
    
    // Ethereum transaction hash
    public string? TransactionId { get; set; }

    public Application(ApplicationType applicationType)
    {
        ApplicationType = applicationType;
        Status = ApplicationStatus.Incomplete;
        Created = DateTime.Now;
    }
}