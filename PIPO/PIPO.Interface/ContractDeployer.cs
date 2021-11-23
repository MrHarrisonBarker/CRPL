using Nethereum.Contracts;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace PIPO.Interface;

public class ContractDeployer<T> : IDisposable where T : ContractDeploymentMessage, new()
{
    private readonly Web3 Web3;
    
    public ContractDeployer(string chainUrl, IAccount account)
    {
        Web3 = new Web3(account, chainUrl);
    }

    public async Task<TransactionReceipt> Deploy(T contractDeployment)
    {
        try
        {
            var deploymentHandler = Web3.Eth.GetContractDeploymentHandler<T>();
            var receipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(contractDeployment);
            
            return receipt;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}