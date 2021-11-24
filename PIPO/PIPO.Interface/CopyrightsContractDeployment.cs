using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace PIPO.Interface;

/// <summary>
/// This class handles the deployment and definition (within .NET) of the copyright contract
/// </summary>
public class CopyrightsContractDeployment : ContractDeploymentMessage
{
    // private CompiledContract CompiledContract;
    
    public CopyrightsContractDeployment() : base(Utils.LoadContract("Copyrights").ByteCode)
    {
    }
}

[Function("simpleMint", "uint256")]
public class SimpleMint : FunctionMessage
{
    [Parameter("address", "owner")]
    public string Owner { get; set; }
}