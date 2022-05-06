using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Applications.Core;

[Owned]
public class ResolveResult
{
    public bool Rejected { get; set; }
    public ResolveStatus ResolvedStatus { get; set; }
    
    // Ethereum transaction hash of the resolving transaction
    public string? Transaction { get; set; }
    public string? Message { get; set; }
}

public enum ResolveStatus
{
    Created,
    NeedsOnChainAction,
    Processing,
    Resolved,
    Failed
}

public class ResolveResultWithUri
{
    public bool Rejected { get; set; }
    public ResolveStatus ResolvedStatus { get; set; }
    public string? Transaction { get; set; }
    public string? TransactionUri { get; set; }
    public string? Message { get; set; }
}