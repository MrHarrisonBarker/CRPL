using CRPL.Data.Applications;

namespace CRPL.Data.Account;

public class RegisteredWork
{
    public RegisteredWork()
    {
        Created = DateTime.Now;
        Status = RegisteredWorkStatus.Created;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    // maps from a bigInt aka uin256
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? RegisteredTransactionId { get; set; }
    public string? ProposalTransactionId { get; set; }
    public List<UserWork> UserWorks { get; set; }
    public List<Application> AssociatedApplication { get; set; }
}