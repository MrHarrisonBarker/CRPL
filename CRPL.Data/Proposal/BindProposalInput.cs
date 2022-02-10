namespace CRPL.Data.Proposal;

public class BindProposalInput
{
    public Guid ApplicationId { get; set; }
    public bool Accepted { get; set; }
}

public class BindProposalWorkInput
{
    public Guid WorkId { get; set; }
    public bool Accepted { get; set; }
}