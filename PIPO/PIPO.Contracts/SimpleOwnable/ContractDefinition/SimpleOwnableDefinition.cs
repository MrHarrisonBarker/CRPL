using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace PIPO.Contracts.SimpleOwnable.ContractDefinition
{


    public partial class SimpleOwnableDeployment : SimpleOwnableDeploymentBase
    {
        public SimpleOwnableDeployment() : base(BYTECODE) { }
        public SimpleOwnableDeployment(string byteCode) : base(byteCode) { }
    }

    public class SimpleOwnableDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public SimpleOwnableDeploymentBase() : base(BYTECODE) { }
        public SimpleOwnableDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class TransferOwnerFunction : TransferOwnerFunctionBase { }

    [Function("transferOwner")]
    public class TransferOwnerFunctionBase : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public virtual string To { get; set; }
    }

    public partial class ChangeOfOwnershipEventDTO : ChangeOfOwnershipEventDTOBase { }

    [Event("ChangeOfOwnership")]
    public class ChangeOfOwnershipEventDTOBase : IEventDTO
    {
        [Parameter("address", "from", 1, false )]
        public virtual string From { get; set; }
        [Parameter("address", "to", 2, false )]
        public virtual string To { get; set; }
    }

    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }


}
