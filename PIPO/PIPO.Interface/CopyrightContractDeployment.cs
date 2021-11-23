using Nethereum.Contracts;

namespace PIPO.Interface;

/// <summary>
/// This class handles the deployment and definition (within .NET) of the copyright contract
/// </summary>
public class CopyrightContractDeployment : ContractDeploymentMessage
{
    // private CompiledContract CompiledContract;
    
    public CopyrightContractDeployment() : base(Utils.LoadContract("Copyright").ByteCode)
    {
    }
}