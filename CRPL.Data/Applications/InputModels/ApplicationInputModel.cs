using CRPL.Data.Applications.ViewModels;
using CRPL.Data.StructuredOwnership;

namespace CRPL.Data.Applications.InputModels;

public abstract class ApplicationInputModel
{
    public Guid Id { get; set; }
}

public class OwnershipRestructureInputModel : ApplicationInputModel
{
    public List<OwnershipStake> CurrentStructure { get; set; }
    public List<OwnershipStake> ProposedStructure { get; set; }
}