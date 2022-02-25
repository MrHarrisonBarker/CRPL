namespace CRPL.Data.Applications.InputModels;

public class ResolveDisputeInputModel
{
    public Guid DisputeId { get; set; }
    public string Message { get; set; }
    public bool Accept { get; set; }
}