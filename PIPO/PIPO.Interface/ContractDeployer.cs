using Nethereum.Contracts;
using Nethereum.RPC.Accounts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace PIPO.Interface;

public class ContractDeployer<T> : IDisposable where T : ContractDeploymentMessage, new()
{
    private readonly BlockChainConnection Connection;
    
    public ContractDeployer(BlockChainConnection connection)
    {
        Connection = connection;
    }

    public async Task<TransactionReceipt> DeployAsync(T contractDeployment)
    {
        try
        {
            var deploymentHandler = Connection.Web3.Eth.GetContractDeploymentHandler<T>();
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
    }
}

public class BlockChainConnection : IDisposable
{
    public Web3 Web3 { get; }

    public BlockChainConnection(string chainUrl, IAccount account)
    {
        Web3 = new Web3(account, chainUrl)
        {
            TransactionManager =
            {
                UseLegacyAsDefault = true
            }
        };
    }
    public void Dispose()
    {
        // throw new NotImplementedException();
    }
}