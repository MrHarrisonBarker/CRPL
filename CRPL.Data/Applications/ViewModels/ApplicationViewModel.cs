using CRPL.Data.Account;
using CRPL.Data.Account.ViewModels;

namespace CRPL.Data.Applications.ViewModels;

public abstract class ApplicationViewModel
{
    public Guid Id { get; set; }
    public ApplicationType ApplicationType { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public string? TransactionId { get; set; }
    public ApplicationStatus Status { get; set; }
    public RegisteredWorkViewModel? AssociatedWork { get; set; }
    public List<UserAccountMinimalViewModel> AssociatedUsers { get; set; }
}

public abstract class ApplicationViewModelWithoutAssociated
{
    public Guid Id { get; set; }
    public string? TransactionId { get; set; }
    public ApplicationType ApplicationType { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public ApplicationStatus Status { get; set; }
}