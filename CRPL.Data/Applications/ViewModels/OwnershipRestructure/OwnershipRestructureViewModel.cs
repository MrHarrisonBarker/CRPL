using CRPL.Data.StructuredOwnership;

namespace CRPL.Data.Applications.ViewModels;

public class OwnershipRestructureViewModel : ApplicationViewModel
{
    public List<OwnershipStake> CurrentStructure { get; set; }
    public List<OwnershipStake> ProposedStructure { get; set; }
    public BindStatus BindStatus { get; set; }
}

public class OwnershipRestructureViewModelWithoutAssociated : ApplicationViewModelWithoutAssociated
{
    
}