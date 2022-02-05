namespace CRPL.Data.Applications;

public class OwnershipRestructureApplication : Application
{
    public OwnershipRestructureApplication(): base(ApplicationType.OwnershipRestructure) {}
    public string CurrentStructure { get; set; }
    public string ProposedStructure { get; set; }
}