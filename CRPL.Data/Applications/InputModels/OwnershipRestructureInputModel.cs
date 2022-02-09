

using CRPL.Data.StructuredOwnership;

namespace CRPL.Data.Applications.InputModels;

public class OwnershipRestructureInputModel : ApplicationInputModel
{
    public Guid? WorkId { get; set; }
    public List<OwnershipStake> CurrentStructure { get; set; }
    public List<OwnershipStake> ProposedStructure { get; set; }
}