using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CRPL.Contracts.Standard.ContractDefinition
{
    public partial class ProposalVote : ProposalVoteBase { }

    public class ProposalVoteBase 
    {
        [Parameter("address", "voter", 1)]
        public virtual string Voter { get; set; }
        [Parameter("bool", "accepted", 2)]
        public virtual bool Accepted { get; set; }
    }
}
