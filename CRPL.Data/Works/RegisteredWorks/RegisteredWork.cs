using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.Workds;

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
    public WorkType WorkType { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    public VerificationResult? VerificationResult { get; set; }
    // maps from a bigInt aka uin256
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? Cid { get; set; }
    public string? RegisteredTransactionId { get; set; }
    public string? ProposalTransactionId { get; set; }
    public List<UserWork> UserWorks { get; set; }
    public List<Application> AssociatedApplication { get; set; }
    
    public long TimesProxyUsed { get; set; }
    public DateTime? LastProxyUse { get; set; }
    
    public long TimesPinged { get; set; }
    public DateTime? LastPing { get; set; }
}