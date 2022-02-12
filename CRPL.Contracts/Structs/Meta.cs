using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CRPL.Contracts.Structs
{
    public partial class Meta : MetaBase { }

    public class MetaBase 
    {
        [Parameter("string", "title", 1)]
        public virtual string Title { get; set; }
        [Parameter("uint256", "expires", 2)]
        public virtual BigInteger Expires { get; set; }
        [Parameter("uint256", "registered", 3)]
        public virtual BigInteger Registered { get; set; }
        [Parameter("string", "workHash", 4)]
        public virtual string WorkHash { get; set; }
        [Parameter("string", "workUri", 5)]
        public virtual string WorkUri { get; set; }
        [Parameter("string", "legalMeta", 6)]
        public virtual string WorkType { get; set; }
        [Parameter("string", "workType", 7)]
        public virtual string LegalMeta { get; set; }
        [Parameter("tuple", "protections", 8)]
        public virtual Protections Protections { get; set; }
    }
}
