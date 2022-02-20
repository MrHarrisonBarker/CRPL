using CRPL.Data.Workds;

namespace CRPL.Data.Account;

public class RegisteredWorkViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    public VerificationResult? VerificationResult { get; set; }
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? RegisteredTransactionId { get; set; }
    public string? RegisteredTransactionUri { get; set; }
}