using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CRPL.Contracts.Standard.ContractDefinition
{
    public partial class OwnershipStakeContract : OwnershipStakeBase { }

    public class OwnershipStakeBase 
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; }
        [Parameter("uint8", "share", 2)]
        public virtual byte Share { get; set; }
    }
}
