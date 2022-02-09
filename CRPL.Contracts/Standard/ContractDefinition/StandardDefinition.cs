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

namespace CRPL.Contracts.Standard.ContractDefinition
{


    public partial class StandardDeployment : StandardDeploymentBase
    {
        public StandardDeployment() : base(BYTECODE) { }
        public StandardDeployment(string byteCode) : base(byteCode) { }
    }

    public class StandardDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "601260809081527114dd185b99185c990810dbdc1e5c9a59da1d60721b60a052610120604052602360c0818152906200236060e039815182906200004b9060019060208401906200006b565b505080516200006290600a9060208401906200006b565b5050506200014e565b828054620000799062000111565b90600052602060002090601f0160209004810192826200009d5760008555620000e8565b82601f10620000b857805160ff1916838001178555620000e8565b82800160010185558215620000e8579182015b82811115620000e8578251825591602001919060010190620000cb565b50620000f6929150620000fa565b5090565b5b80821115620000f65760008155600101620000fb565b600181811c908216806200012657607f821691505b602082108114156200014857634e487b7160e01b600052602260045260246000fd5b50919050565b612202806200015e6000396000f3fe6080604052600436106101095760003560e01c806358febe2a11610095578063ad1bdc4311610064578063ad1bdc4314610341578063e86bd09314610356578063ea6804c214610369578063edc288bd14610389578063ff218f42146103a957600080fd5b806358febe2a1461026b578063927a80c11461029857806399226076146102b8578063ab510e951461031157600080fd5b8063383ad2fa116100dc578063383ad2fa146101cb5780633ad5ed1a146101f85780634d44ee1a146102185780634edaeb401461023857806357faa4501461024b57600080fd5b80630dcae7221461010e578063104b4f2614610123578063141cddab146101665780632aea379b1461019e575b600080fd5b61012161011c366004611cfa565b6103c9565b005b34801561012f57600080fd5b5061015361013e366004611d41565b6000908152600b602052604090206002015490565b6040519081526020015b60405180910390f35b34801561017257600080fd5b50610186610181366004611d41565b6107a3565b6040516001600160a01b03909116815260200161015d565b3480156101aa57600080fd5b506101be6101b9366004611d41565b610859565b60405161015d9190611d5a565b3480156101d757600080fd5b506101eb6101e6366004611d41565b6108fe565b60405161015d9190611e04565b34801561020457600080fd5b506101be610213366004611d41565b6109b7565b34801561022457600080fd5b50610121610233366004611e56565b6109d7565b610121610246366004611e89565b610a91565b34801561025757600080fd5b50610153610266366004611eac565b610cd6565b34801561027757600080fd5b5061028b610286366004611d41565b610d43565b60405161015d9190611ec7565b3480156102a457600080fd5b506101216102b3366004611f95565b610e59565b3480156102c457600080fd5b506103016102d33660046120a9565b6001600160a01b03918216600090815260086020908152604080832093909416825291909152205460ff1690565b604051901515815260200161015d565b34801561031d57600080fd5b5061015361032c366004611d41565b6000908152600b602052604090206001015490565b34801561034d57600080fd5b506101be610efc565b6101216103643660046120d3565b610f8e565b34801561037557600080fd5b506101be610384366004611d41565b611325565b34801561039557600080fd5b506101be6103a4366004611d41565b611345565b3480156103b557600080fd5b506101216103c43660046120f6565b611362565b6000828152600260205260409020548290158061041757506000818152600260205260408120805482906103ff576103ff612133565b6000918252602090912001546001600160a01b031614155b6040518060400160405280600f81526020016e1393d517d59053125117d49251d215608a1b815250906104665760405162461bcd60e51b815260040161045d9190611d5a565b60405180910390fd5b508160008151116040518060400160405280600f81526020016e4e4f5f5348415245484f4c4445525360881b815250906104b35760405162461bcd60e51b815260040161045d9190611d5a565b5060005b81518160ff1610156105505760006001600160a01b0316828260ff16815181106104e3576104e3612133565b6020026020010151600001516001600160a01b031614156040518060400160405280600c81526020016b24a72b20a624a22fa0a2222960a11b8152509061053d5760405162461bcd60e51b815260040161045d9190611d5a565b50806105488161215f565b9150506104b7565b5083336000805b60008481526002602052604090205460ff821610156105da57600084815260026020526040902080546001600160a01b038516919060ff841690811061059f5761059f612133565b6000918252602090912001546001600160a01b031614156105c857816105c48161215f565b9250505b806105d28161215f565b915050610557565b506000838152600760205260409020546001600160a01b038381169116141561060b57806106078161215f565b9150505b60408051808201909152600f81526e2727aa2fa9a420a922a427a62222a960891b6020820152600160ff8316146106555760405162461bcd60e51b815260040161045d9190611d5a565b5060005b86518160ff16101561075a576000878260ff168151811061067c5761067c612133565b60200260200101516020015160ff16116040518060400160405280600d81526020016c494e56414c49445f534841524560981b815250906106d05760405162461bcd60e51b815260040161045d9190611d5a565b5060008881526004602052604090208751889060ff84169081106106f6576106f6612133565b6020908102919091018101518254600181018455600093845292829020815193018054919092015160ff16600160a01b026001600160a81b03199091166001600160a01b0390931692909217919091179055806107528161215f565b915050610659565b50867e8009484f66f16af1f8aef773b8ad0526448b2b1fae996ea810cd9e09c3378961078589611632565b6040516107929190611e04565b60405180910390a250505050505050565b600081815260026020526040812054829015806107f157506000818152600260205260408120805482906107d9576107d9612133565b6000918252602090912001546001600160a01b031614155b6040518060400160405280600f81526020016e1393d517d59053125117d49251d215608a1b815250906108375760405162461bcd60e51b815260040161045d9190611d5a565b506000838152600760205260409020546001600160a01b031691505b50919050565b6000818152600b602052604090206005018054606091906108799061217f565b80601f01602080910402602001604051908101604052809291908181526020018280546108a59061217f565b80156108f25780601f106108c7576101008083540402835291602001916108f2565b820191906000526020600020905b8154815290600101906020018083116108d557829003601f168201915b50505050509050919050565b604080518082019091526060808252602082015260008281526002602052604090205482901580610960575060008181526002602052604081208054829061094857610948612133565b6000918252602090912001546001600160a01b031614155b6040518060400160405280600f81526020016e1393d517d59053125117d49251d215608a1b815250906109a65760405162461bcd60e51b815260040161045d9190611d5a565b506109b083611632565b9392505050565b6000818152600b602052604090206004018054606091906108799061217f565b60408051808201909152600c81526b24a72b20a624a22fa0a2222960a11b602082015282906001600160a01b038216610a235760405162461bcd60e51b815260040161045d9190611d5a565b503360008181526008602090815260408083206001600160a01b03881680855290835292819020805460ff191687151590811790915590519081529192917ff6ce41d6d3d4433d03c2eb7f54a6ecb9abda9f1099e307af2fcf6e58fcda8445910160405180910390a3505050565b60008281526002602052604090205482901580610adf5750600081815260026020526040812080548290610ac757610ac7612133565b6000918252602090912001546001600160a01b031614155b6040518060400160405280600f81526020016e1393d517d59053125117d49251d215608a1b81525090610b255760405162461bcd60e51b815260040161045d9190611d5a565b5060408051808201909152600c81526b24a72b20a624a22fa0a2222960a11b602082015282906001600160a01b038216610b725760405162461bcd60e51b815260040161045d9190611d5a565b5083336000805b60008481526002602052604090205460ff82161015610bfc57600084815260026020526040902080546001600160a01b038516919060ff8416908110610bc157610bc1612133565b6000918252602090912001546001600160a01b03161415610bea5781610be68161215f565b9250505b80610bf48161215f565b915050610b79565b506000838152600760205260409020546001600160a01b0383811691161415610c2d5780610c298161215f565b9150505b60408051808201909152600f81526e2727aa2fa9a420a922a427a62222a960891b6020820152600160ff831614610c775760405162461bcd60e51b815260040161045d9190611d5a565b5060008781526007602052604080822080546001600160a01b0319166001600160a01b038a169081179091559051909189917f7b39c92a7e1a86e846edaeff6eba715a046352c596794c2a374269c126a997689190a350505050505050565b60408051808201909152600c81526b24a72b20a624a22fa0a2222960a11b602082015260009082906001600160a01b038216610d255760405162461bcd60e51b815260040161045d9190611d5a565b5050506001600160a01b031660009081526003602052604090205490565b60008181526002602052604090205460609082901580610d945750600081815260026020526040812080548290610d7c57610d7c612133565b6000918252602090912001546001600160a01b031614155b6040518060400160405280600f81526020016e1393d517d59053125117d49251d215608a1b81525090610dda5760405162461bcd60e51b815260040161045d9190611d5a565b50600083815260026020908152604080832080548251818502810185019093528083529193909284015b82821015610e4d57600084815260209081902060408051808201909152908401546001600160a01b0381168252600160a01b900460ff1681830152825260019092019101610e04565b50505050915050919050565b6000610e648361174e565b6000818152600b602090815260409091208451805193945085939192610e8f92849290910190611a30565b5060208281015160018301556040830151600283015560608301518051610ebc9260038501920190611a30565b5060808201518051610ed8916004840191602090910190611a30565b5060a08201518051610ef4916005840191602090910190611a30565b505050505050565b6060600a8054610f0b9061217f565b80601f0160208091040260200160405190810160405280929190818152602001828054610f379061217f565b8015610f845780601f10610f5957610100808354040283529160200191610f84565b820191906000526020600020905b815481529060010190602001808311610f6757829003601f168201915b5050505050905090565b81336000805b60008481526002602052604090205460ff8216101561101757600084815260026020526040902080546001600160a01b038516919060ff8416908110610fdc57610fdc612133565b6000918252602090912001546001600160a01b0316141561100557816110018161215f565b9250505b8061100f8161215f565b915050610f94565b506000838152600760205260409020546001600160a01b038381169116141561104857806110448161215f565b9150505b60408051808201909152600f81526e2727aa2fa9a420a922a427a62222a960891b6020820152600160ff8316146110925760405162461bcd60e51b815260040161045d9190611d5a565b50600085815260026020526040902054859015806110e157506000818152600260205260408120805482906110c9576110c9612133565b6000918252602090912001546001600160a01b031614155b6040518060400160405280600f81526020016e1393d517d59053125117d49251d215608a1b815250906111275760405162461bcd60e51b815260040161045d9190611d5a565b50611132863361184b565b6000868152600560209081526040808320815180830183523381528915158185019081528254600181018455928652848620915191909201805492511515600160a01b026001600160a81b03199093166001600160a01b03929092169190911791909117905588835260069091528120805460ff16916111b18361215f565b91906101000a81548160ff021916908360ff1602179055505060005b60008781526005602052604090205460ff8216101561126f576000878152600560205260409020805460ff831690811061120957611209612133565b600091825260209091200154600160a01b900460ff1661125d5761122c8761190d565b60405187907f68bb464d4f2f96670fc0aed6a9a8666f649171cd91a42b0d951f98636dbf4cbd90600090a250610ef4565b806112678161215f565b9150506111cd565b5060008681526002602052604090205460008781526006602052604090205460ff161415610ef4576112a08661190d565b6000868152600460209081526040808320600290925290912081546112c59290611ab4565b5060008681526004602052604081206112dd91611b3b565b857fa3d85778fe4bc6d35bb5cbd45b8f87741b9c7dd921cba7c583627aea57835d0561130888611632565b6040516113159190611e04565b60405180910390a2505050505050565b6000818152600b602052604090206003018054606091906108799061217f565b6000818152600b602052604090208054606091906108799061217f565b8060008151116040518060400160405280600f81526020016e4e4f5f5348415245484f4c4445525360881b815250906113ae5760405162461bcd60e51b815260040161045d9190611d5a565b5060005b81518160ff16101561144b5760006001600160a01b0316828260ff16815181106113de576113de612133565b6020026020010151600001516001600160a01b031614156040518060400160405280600c81526020016b24a72b20a624a22fa0a2222960a11b815250906114385760405162461bcd60e51b815260040161045d9190611d5a565b50806114438161215f565b9150506113b2565b50600061145860096119da565b905060005b83518160ff161015611587576000848260ff168151811061148057611480612133565b60200260200101516020015160ff16116040518060400160405280600d81526020016c494e56414c49445f534841524560981b815250906114d45760405162461bcd60e51b815260040161045d9190611d5a565b506114fe848260ff16815181106114ed576114ed612133565b6020026020010151600001516119ea565b60008281526002602052604090208451859060ff841690811061152357611523612133565b6020908102919091018101518254600181018455600093845292829020815193018054919092015160ff16600160a01b026001600160a81b03199091166001600160a01b03909316929092179190911790558061157f8161215f565b91505061145d565b50336007600061159660095490565b815260200190815260200160002060006101000a8154816001600160a01b0302191690836001600160a01b03160217905550807f4bd6f579ae7c434c3241f34c77172a9b72a35649989f42837cf09152f277fa3d846040516115f89190611ec7565b60405180910390a2604051339082907f7b39c92a7e1a86e846edaeff6eba715a046352c596794c2a374269c126a9976890600090a3505050565b60408051808201825260608082526020808301829052835160008681526002835285812080549384028301850187529582018381529495919485949093919085015b828210156116bd57600084815260209081902060408051808201909152908401546001600160a01b0381168252600160a01b900460ff1681830152825260019092019101611674565b50505050815260200160046000858152602001908152602001600020805480602002602001604051908101604052809291908181526020016000905b8282101561174257600084815260209081902060408051808201909152908401546001600160a01b0381168252600160a01b900460ff16818301528252600190920191016116f9565b50505091525092915050565b60008160008151116040518060400160405280600f81526020016e4e4f5f5348415245484f4c4445525360881b8152509061179c5760405162461bcd60e51b815260040161045d9190611d5a565b5060005b81518160ff1610156118395760006001600160a01b0316828260ff16815181106117cc576117cc612133565b6020026020010151600001516001600160a01b031614156040518060400160405280600c81526020016b24a72b20a624a22fa0a2222960a11b815250906118265760405162461bcd60e51b815260040161045d9190611d5a565b50806118318161215f565b9150506117a0565b5061184383611362565b6009546109b0565b60005b60008381526005602052604090205460ff8216101561190857600083815260056020526040902080546001600160a01b038416919060ff841690811061189657611896612133565b6000918252602091829020015460408051808201909152600d81526c1053149150511657d593d51151609a1b9281019290925290916001600160a01b0390911614156118f55760405162461bcd60e51b815260040161045d9190611d5a565b50806119008161215f565b91505061184e565b505050565b600081815260026020908152604080832080548251818502810185019093528083529192909190849084015b8282101561198257600084815260209081902060408051808201909152908401546001600160a01b0381168252600160a01b900460ff1681830152825260019092019101611939565b50505050905060005b81518160ff1610156119c05760008381526005602052604081206119ae91611b3b565b806119b88161215f565b91505061198b565b50506000908152600660205260409020805460ff19169055565b60006119e582611a1b565b505490565b6001600160a01b0381166000908152600360205260408120805460019290611a139084906121b4565b909155505050565b6001816000016000828254611a1391906121b4565b828054611a3c9061217f565b90600052602060002090601f016020900481019282611a5e5760008555611aa4565b82601f10611a7757805160ff1916838001178555611aa4565b82800160010185558215611aa4579182015b82811115611aa4578251825591602001919060010190611a89565b50611ab0929150611b5c565b5090565b828054828255906000526020600020908101928215611b2f5760005260206000209182015b82811115611b2f57825482546001600160a01b039091166001600160a01b031982168117845584546001600160a81b031990921617600160a01b9182900460ff1690910217825560019283019290910190611ad9565b50611ab0929150611b71565b5080546000825590600052602060002090810190611b599190611b71565b50565b5b80821115611ab05760008155600101611b5d565b5b80821115611ab05780546001600160a81b0319168155600101611b72565b634e487b7160e01b600052604160045260246000fd5b6040805190810167ffffffffffffffff81118282101715611bc957611bc9611b90565b60405290565b60405160c0810167ffffffffffffffff81118282101715611bc957611bc9611b90565b604051601f8201601f1916810167ffffffffffffffff81118282101715611c1b57611c1b611b90565b604052919050565b80356001600160a01b0381168114611c3a57600080fd5b919050565b600082601f830112611c5057600080fd5b8135602067ffffffffffffffff821115611c6c57611c6c611b90565b611c7a818360051b01611bf2565b82815260069290921b84018101918181019086841115611c9957600080fd5b8286015b84811015611cef5760408189031215611cb65760008081fd5b611cbe611ba6565b611cc782611c23565b81528482013560ff81168114611cdd5760008081fd5b81860152835291830191604001611c9d565b509695505050505050565b60008060408385031215611d0d57600080fd5b82359150602083013567ffffffffffffffff811115611d2b57600080fd5b611d3785828601611c3f565b9150509250929050565b600060208284031215611d5357600080fd5b5035919050565b600060208083528351808285015260005b81811015611d8757858101830151858201604001528201611d6b565b81811115611d99576000604083870101525b50601f01601f1916929092016040019392505050565b600081518084526020808501945080840160005b83811015611df957815180516001600160a01b0316885260209081015160ff169088015260408701965090820190600101611dc3565b509495945050505050565b602081526000825160406020840152611e206060840182611daf565b90506020840151601f19848303016040850152611e3d8282611daf565b95945050505050565b80358015158114611c3a57600080fd5b60008060408385031215611e6957600080fd5b611e7283611c23565b9150611e8060208401611e46565b90509250929050565b60008060408385031215611e9c57600080fd5b82359150611e8060208401611c23565b600060208284031215611ebe57600080fd5b6109b082611c23565b6020808252825182820181905260009190848201906040850190845b81811015611f1957835180516001600160a01b0316845260209081015160ff169084015260408301938501939250600101611ee3565b50909695505050505050565b600082601f830112611f3657600080fd5b813567ffffffffffffffff811115611f5057611f50611b90565b611f63601f8201601f1916602001611bf2565b818152846020838601011115611f7857600080fd5b816020850160208301376000918101602001919091529392505050565b60008060408385031215611fa857600080fd5b823567ffffffffffffffff80821115611fc057600080fd5b611fcc86838701611c3f565b93506020850135915080821115611fe257600080fd5b9084019060c08287031215611ff657600080fd5b611ffe611bcf565b82358281111561200d57600080fd5b61201988828601611f25565b825250602083013560208201526040830135604082015260608301358281111561204257600080fd5b61204e88828601611f25565b60608301525060808301358281111561206657600080fd5b61207288828601611f25565b60808301525060a08301358281111561208a57600080fd5b61209688828601611f25565b60a0830152508093505050509250929050565b600080604083850312156120bc57600080fd5b6120c583611c23565b9150611e8060208401611c23565b600080604083850312156120e657600080fd5b82359150611e8060208401611e46565b60006020828403121561210857600080fd5b813567ffffffffffffffff81111561211f57600080fd5b61212b84828501611c3f565b949350505050565b634e487b7160e01b600052603260045260246000fd5b634e487b7160e01b600052601160045260246000fd5b600060ff821660ff81141561217657612176612149565b60010192915050565b600181811c9082168061219357607f821691505b6020821081141561085357634e487b7160e01b600052602260045260246000fd5b600082198211156121c7576121c7612149565b50019056fea264697066735822122045fdd09f1662d98f6bf15b11f4e79b54732b0007de07c8defc79129d9abb0b0464736f6c634300080b00335374616e6461726420436f70797269676874206c6567616c20646566696e6974696f6e";
        public StandardDeploymentBase() : base(BYTECODE) { }
        public StandardDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class ApproveManagerFunction : ApproveManagerFunctionBase { }

    [Function("ApproveManager")]
    public class ApproveManagerFunctionBase : FunctionMessage
    {
        [Parameter("address", "manager", 1)]
        public virtual string Manager { get; set; }
        [Parameter("bool", "hasApproval", 2)]
        public virtual bool HasApproval { get; set; }
    }

    public partial class ApproveOneFunction : ApproveOneFunctionBase { }

    [Function("ApproveOne")]
    public class ApproveOneFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
        [Parameter("address", "approved", 2)]
        public virtual string Approved { get; set; }
    }

    public partial class BindRestructureFunction : BindRestructureFunctionBase { }

    [Function("BindRestructure")]
    public class BindRestructureFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
        [Parameter("bool", "accepted", 2)]
        public virtual bool Accepted { get; set; }
    }

    public partial class ExpiresOnFunction : ExpiresOnFunctionBase { }

    [Function("ExpiresOn", "uint256")]
    public class ExpiresOnFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class GetApprovedFunction : GetApprovedFunctionBase { }

    [Function("GetApproved", "address")]
    public class GetApprovedFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class IsManagerFunction : IsManagerFunctionBase { }

    [Function("IsManager", "bool")]
    public class IsManagerFunctionBase : FunctionMessage
    {
        [Parameter("address", "client", 1)]
        public virtual string Client { get; set; }
        [Parameter("address", "manager", 2)]
        public virtual string Manager { get; set; }
    }

    public partial class LegalDefinitionFunction : LegalDefinitionFunctionBase { }

    [Function("LegalDefinition", "string")]
    public class LegalDefinitionFunctionBase : FunctionMessage
    {

    }

    public partial class LegalMetaFunction : LegalMetaFunctionBase { }

    [Function("LegalMeta", "string")]
    public class LegalMetaFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class OwnershipOfFunction : OwnershipOfFunctionBase { }

    [Function("OwnershipOf", typeof(OwnershipOfOutputDTO))]
    public class OwnershipOfFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class PortfolioSizeFunction : PortfolioSizeFunctionBase { }

    [Function("PortfolioSize", "uint256")]
    public class PortfolioSizeFunctionBase : FunctionMessage
    {
        [Parameter("address", "owner", 1)]
        public virtual string Owner { get; set; }
    }

    public partial class ProposalFunction : ProposalFunctionBase { }

    [Function("Proposal", typeof(ProposalOutputDTO))]
    public class ProposalFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class ProposeRestructureFunction : ProposeRestructureFunctionBase { }

    [Function("ProposeRestructure")]
    public class ProposeRestructureFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
        [Parameter("tuple[]", "restructured", 2)]
        public virtual List<OwnershipStakeContract> Restructured { get; set; }
    }

    public partial class RegisterFunction : RegisterFunctionBase { }

    [Function("Register")]
    public class RegisterFunctionBase : FunctionMessage
    {
        [Parameter("tuple[]", "to", 1)]
        public virtual List<OwnershipStakeContract> To { get; set; }
    }

    public partial class RegisterTimeFunction : RegisterTimeFunctionBase { }

    [Function("RegisterTime", "uint256")]
    public class RegisterTimeFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class RegisterWithMetaFunction : RegisterWithMetaFunctionBase { }

    [Function("RegisterWithMeta")]
    public class RegisterWithMetaFunctionBase : FunctionMessage
    {
        [Parameter("tuple[]", "to", 1)]
        public virtual List<OwnershipStakeContract> To { get; set; }
        [Parameter("tuple", "def", 2)]
        public virtual Meta Def { get; set; }
    }

    public partial class TitleFunction : TitleFunctionBase { }

    [Function("Title", "string")]
    public class TitleFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class WorkHashFunction : WorkHashFunctionBase { }

    [Function("WorkHash", "string")]
    public class WorkHashFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class WorkURIFunction : WorkURIFunctionBase { }

    [Function("WorkURI", "string")]
    public class WorkURIFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "rightId", 1)]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class ApprovedEventDTO : ApprovedEventDTOBase { }

    [Event("Approved")]
    public class ApprovedEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
        [Parameter("address", "approved", 2, true )]
        public virtual string Approved { get; set; }
    }

    public partial class ApprovedManagerEventDTO : ApprovedManagerEventDTOBase { }

    [Event("ApprovedManager")]
    public class ApprovedManagerEventDTOBase : IEventDTO
    {
        [Parameter("address", "owner", 1, true )]
        public virtual string Owner { get; set; }
        [Parameter("address", "manager", 2, true )]
        public virtual string Manager { get; set; }
        [Parameter("bool", "hasApproval", 3, false )]
        public virtual bool HasApproval { get; set; }
    }

    public partial class DisputedEventDTO : DisputedEventDTOBase { }

    [Event("Disputed")]
    public class DisputedEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
        [Parameter("address", "by", 2, true )]
        public virtual string By { get; set; }
        [Parameter("bytes", "reason", 3, false )]
        public virtual byte[] Reason { get; set; }
    }

    public partial class FailedProposalEventDTO : FailedProposalEventDTOBase { }

    [Event("FailedProposal")]
    public class FailedProposalEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
    }

    public partial class ModifyEventDTO : ModifyEventDTOBase { }

    [Event("Modify")]
    public class ModifyEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
        [Parameter("bytes", "modification", 2, false )]
        public virtual byte[] Modification { get; set; }
    }

    public partial class ProposedRestructureEventDTO : ProposedRestructureEventDTOBase { }

    [Event("ProposedRestructure")]
    public class ProposedRestructureEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
        [Parameter("tuple", "proposal", 2, false )]
        public virtual RestructureProposal Proposal { get; set; }
    }

    public partial class RegisteredEventDTO : RegisteredEventDTOBase { }

    [Event("Registered")]
    public class RegisteredEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
        [Parameter("tuple[]", "to", 2, false )]
        public virtual List<OwnershipStakeContract> To { get; set; }
    }

    public partial class RestructuredEventDTO : RestructuredEventDTOBase { }

    [Event("Restructured")]
    public class RestructuredEventDTOBase : IEventDTO
    {
        [Parameter("uint256", "rightId", 1, true )]
        public virtual BigInteger RightId { get; set; }
        [Parameter("tuple", "proposal", 2, false )]
        public virtual RestructureProposal Proposal { get; set; }
    }







    public partial class ExpiresOnOutputDTO : ExpiresOnOutputDTOBase { }

    [FunctionOutput]
    public class ExpiresOnOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetApprovedOutputDTO : GetApprovedOutputDTOBase { }

    [FunctionOutput]
    public class GetApprovedOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class IsManagerOutputDTO : IsManagerOutputDTOBase { }

    [FunctionOutput]
    public class IsManagerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class LegalDefinitionOutputDTO : LegalDefinitionOutputDTOBase { }

    [FunctionOutput]
    public class LegalDefinitionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class LegalMetaOutputDTO : LegalMetaOutputDTOBase { }

    [FunctionOutput]
    public class LegalMetaOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class OwnershipOfOutputDTO : OwnershipOfOutputDTOBase { }

    [FunctionOutput]
    public class OwnershipOfOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple[]", "", 1)]
        public virtual List<OwnershipStakeContract> ReturnValue1 { get; set; }
    }

    public partial class PortfolioSizeOutputDTO : PortfolioSizeOutputDTOBase { }

    [FunctionOutput]
    public class PortfolioSizeOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class ProposalOutputDTO : ProposalOutputDTOBase { }

    [FunctionOutput]
    public class ProposalOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple", "", 1)]
        public virtual RestructureProposal ReturnValue1 { get; set; }
    }





    public partial class RegisterTimeOutputDTO : RegisterTimeOutputDTOBase { }

    [FunctionOutput]
    public class RegisterTimeOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }



    public partial class TitleOutputDTO : TitleOutputDTOBase { }

    [FunctionOutput]
    public class TitleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class WorkHashOutputDTO : WorkHashOutputDTOBase { }

    [FunctionOutput]
    public class WorkHashOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    public partial class WorkURIOutputDTO : WorkURIOutputDTOBase { }

    [FunctionOutput]
    public class WorkURIOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }
}
