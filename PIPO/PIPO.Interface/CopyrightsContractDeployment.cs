using Nethereum.Contracts;

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