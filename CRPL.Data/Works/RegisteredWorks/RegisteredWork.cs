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
    public string? RightId { get; set; }
    
    // Hash of the uploaded work
    public byte[]? Hash { get; set; }
    
    // Unique id used by IPFS to retrieve files from the network
    public string? Cid { get; set; }
    
    // Work registration transaction hash
    public string? RegisteredTransactionId { get; set; }
    
    // Latest proposal transaction hash
    public string? ProposalTransactionId { get; set; }
    
    // Many to many, User <-> RegisteredWork
    public List<UserWork> UserWorks { get; set; }
    
    // Many to many, User <-> Application
    public List<Application> AssociatedApplication { get; set; }
    
    public long TimesProxyUsed { get; set; }
    public DateTime? LastProxyUse { get; set; }
    
    public long TimesPinged { get; set; }
    public DateTime? LastPing { get; set; }
}