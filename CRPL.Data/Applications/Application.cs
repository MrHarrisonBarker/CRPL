using CRPL.Data.Account;

namespace CRPL.Data.Applications;

public class Application
{
    public Guid Id { get; set; }

    public ApplicationType ApplicationType { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    public List<PartialField>? Fields { get; set; }
    public List<UserApplication> AssociatedUsers { get; set; }
}

public class UserApplication
{
    public Guid ApplicationId { get; set; }
    public Application Application { get; set; }
    public Guid UserId { get; set; }
    public UserAccount UserAccount { get; set; }
}

public enum ApplicationType
{
    CopyrightRegistration,
    OwnershipRestructure,
    CopyrightTypeChange,
    Dispute
}