using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;

namespace CRPL.Data.Account;

public class RegisteredWork
{
    public RegisteredWork()
    {
        Created = DateTime.Now;
        Status = RegisteredWorkStatus.Created;
    }

    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    // maps from a bigInt aka uin256
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? RegisteredTransactionId { get; set; }
    public List<UserWork> UserWorks { get; set; }
    public List<Application> AssociatedApplication { get; set; }
}

public class RegisteredWorkWithAppsViewModel
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? RegisteredTransactionId { get; set; }
    public List<ApplicationViewModelWithoutAssociated> AssociatedApplication { get; set; }
}