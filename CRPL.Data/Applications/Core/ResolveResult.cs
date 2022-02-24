using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Applications.Core;

[Owned]
public class ResolveResult
{
    public bool Rejected { get; set; }
    public string? Transaction { get; set; }
    public string? Message { get; set; }
}

public class ResolveResultWithUri
{
    public bool Rejected { get; set; }
    public string? Transaction { get; set; }
    public string? TransactionUri { get; set; }
    public string? Message { get; set; }
}