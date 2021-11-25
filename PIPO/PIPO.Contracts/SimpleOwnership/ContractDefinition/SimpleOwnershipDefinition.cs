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

namespace PIPO.Contracts.SimpleOwnership.ContractDefinition
{


    public partial class SimpleOwnershipDeployment : SimpleOwnershipDeploymentBase
    {
        public SimpleOwnershipDeployment() : base(BYTECODE) { }
        public SimpleOwnershipDeployment(string byteCode) : base(byteCode) { }
    }

    public class SimpleOwnershipDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b5060405161043938038061043983398101604081905261002f9161010a565b600080546001600160a01b03191633179055805161005490600290602084019061005b565b5050610214565b828054610067906101d9565b90600052602060002090601f01602090048101928261008957600085556100cf565b82601f106100a257805160ff19168380011785556100cf565b828001600101855582156100cf579182015b828111156100cf5782518255916020019190600101906100b4565b506100db9291506100df565b5090565b5b808211156100db57600081556001016100e0565b634e487b7160e01b600052604160045260246000fd5b6000602080838503121561011d57600080fd5b82516001600160401b038082111561013457600080fd5b818501915085601f83011261014857600080fd5b81518181111561015a5761015a6100f4565b604051601f8201601f19908116603f01168101908382118183101715610182576101826100f4565b81604052828152888684870101111561019a57600080fd5b600093505b828410156101bc578484018601518185018701529285019261019f565b828411156101cd5760008684830101525b98975050505050505050565b600181811c908216806101ed57607f821691505b6020821081141561020e57634e487b7160e01b600052602260045260246000fd5b50919050565b610216806102236000396000f3fe608060405234801561001057600080fd5b506004361061004c5760003560e01c80634fb2e45d1461005157806358dd2718146100665780638da5cb5b14610079578063c8bec4f714610099575b600080fd5b61006461005f366004610187565b6100aa565b005b610064610074366004610187565b610166565b6000546040516001600160a01b0390911681526020015b60405180910390f35b600154604051908152602001610090565b6000546001600160a01b031633146101015760405162461bcd60e51b8152602060048201526016602482015275596f7520617265206e6f7420746865206f776e65722160501b604482015260640160405180910390fd5b600054604080516001600160a01b03928316815291831660208301527fd68075de5cc16fff300716f78dd84dac449d7b88d17e4921d6cf6caa33347641910160405180910390a1600080546001600160a01b0319166001600160a01b03831617905550565b61016f816100aa565b6001805490600061017f836101b7565b919050555050565b60006020828403121561019957600080fd5b81356001600160a01b03811681146101b057600080fd5b9392505050565b60006000198214156101d957634e487b7160e01b600052601160045260246000fd5b506001019056fea26469706673582212202653be87ee69d19d53a99b3e0cda40f73ee9b4eed6dd053545f3833d220945c064736f6c634300080a0033";
        public SimpleOwnershipDeploymentBase() : base(BYTECODE) { }
        public SimpleOwnershipDeploymentBase(string byteCode) : base(byteCode) { }
        [Parameter("string", "name", 1)]
        public virtual string Name { get; set; }
    }

    public partial class NumberOfOwnershipsFunction : NumberOfOwnershipsFunctionBase { }

    [Function("numberOfOwnerships", "uint256")]
    public class NumberOfOwnershipsFunctionBase : FunctionMessage
    {

    }

    public partial class OwnerFunction : OwnerFunctionBase { }

    [Function("owner", "address")]
    public class OwnerFunctionBase : FunctionMessage
    {

    }

    public partial class SimpleMintFunction : SimpleMintFunctionBase { }

    [Function("simpleMint")]
    public class SimpleMintFunctionBase : FunctionMessage
    {
        [Parameter("address", "receiver", 1)]
        public virtual string Receiver { get; set; }
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

    public partial class NumberOfOwnershipsOutputDTO : NumberOfOwnershipsOutputDTOBase { }

    [FunctionOutput]
    public class NumberOfOwnershipsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class OwnerOutputDTO : OwnerOutputDTOBase { }

    [FunctionOutput]
    public class OwnerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }




}
