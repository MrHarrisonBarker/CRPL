using System.Collections.Generic;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CRPL.Contracts.Standard.ContractDefinition
{
    public partial class RestructureProposal : RestructureProposalBase { }

    public class RestructureProposalBase 
    {
        [Parameter("tuple[]", "oldStructure", 1)]
        public virtual List<OwnershipStakeContract> OldStructure { get; set; }
        [Parameter("tuple[]", "newStructure", 2)]
        public virtual List<OwnershipStakeContract> NewStructure { get; set; }
    }
}
