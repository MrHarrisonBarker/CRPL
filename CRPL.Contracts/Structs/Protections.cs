using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CRPL.Contracts.Structs
{
    [Owned]
    public partial class Protections : ProtectionsBase { }

    public class ProtectionsBase 
    {
        [Parameter("bool", "authorship", 1)]
        public virtual bool Authorship { get; set; }
        [Parameter("bool", "commercialAdaptation", 2)]
        public virtual bool CommercialAdaptation { get; set; }
        [Parameter("bool", "nonCommercialAdaptation", 3)]
        public virtual bool NonCommercialAdaptation { get; set; }
        [Parameter("bool", "reviewOrCrit", 4)]
        public virtual bool ReviewOrCrit { get; set; }
        [Parameter("bool", "commercialPerformance", 5)]
        public virtual bool CommercialPerformance { get; set; }
        [Parameter("bool", "nonCommercialPerformance", 6)]
        public virtual bool NonCommercialPerformance { get; set; }
        [Parameter("bool", "commercialReproduction", 7)]
        public virtual bool CommercialReproduction { get; set; }
        [Parameter("bool", "nonCommercialReproduction", 8)]
        public virtual bool NonCommercialReproduction { get; set; }
        [Parameter("bool", "commercialDistribution", 9)]
        public virtual bool CommercialDistribution { get; set; }
        [Parameter("bool", "nonCommercialDistribution", 10)]
        public virtual bool NonCommercialDistribution { get; set; }
    }
}
