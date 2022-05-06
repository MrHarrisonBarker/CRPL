namespace CRPL.Data.Applications;

public class OwnershipRestructureApplication : Application
{
    public OwnershipRestructureApplication(): base(ApplicationType.OwnershipRestructure) {}
    
    // The structures complex so are encoded into a string for easily storing in the database avoiding another relationship
    public string CurrentStructure { get; set; }
    public string ProposedStructure { get; set; }
    public BindStatus BindStatus { get; set; } = BindStatus.NoProposal;
    
    // Used for disputes propagating changes back to the original dispute application 
    public RestructureReason RestructureReason { get; set; }
    
    // If the application has been made by another (by a dispute resolving) a relationship is made
    public Application? Origin { get; set; }
}

public enum RestructureReason
{
    Application,
    Dispute,
    DeleteAccount,
    TransferWallet
}