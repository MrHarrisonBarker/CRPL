namespace CRPL.Data.Applications;

public class OwnershipRestructureApplication : Application
{
    public OwnershipRestructureApplication(): base(ApplicationType.OwnershipRestructure) {}
    public string CurrentStructure { get; set; }
    public string ProposedStructure { get; set; }
    public BindStatus BindStatus { get; set; } = BindStatus.NoProposal;
    public RestructureReason RestructureReason { get; set; }
    public Application? Origin { get; set; }
}

public enum RestructureReason
{
    Application,
    Dispute,
    DeleteAccount,
    TransferWallet
}