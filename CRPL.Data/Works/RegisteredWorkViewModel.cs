namespace CRPL.Data.Account;

public class RegisteredWorkViewModel
{
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Registered { get; set; }
    public RegisteredWorkStatus Status { get; set; }
    public string? RightId { get; set; }
    public byte[]? Hash { get; set; }
    public string? RegisteredTransactionId { get; set; }
}