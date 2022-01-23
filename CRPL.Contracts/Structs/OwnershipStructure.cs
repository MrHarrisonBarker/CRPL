using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CRPL.Contracts.Structs
{
    public partial class OwnershipStructure : OwnershipStructureBase { }

    public class OwnershipStructureBase 
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; }
        [Parameter("uint8", "share", 2)]
        public virtual byte Share { get; set; }
    }
}
