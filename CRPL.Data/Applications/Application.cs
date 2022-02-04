namespace CRPL.Data.Applications;

public abstract class Application
{
    public Guid Id { get; set; }

    public ApplicationType ApplicationType { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public ApplicationStatus Status { get; set; }
    public List<UserApplication> AssociatedUsers { get; set; }
}