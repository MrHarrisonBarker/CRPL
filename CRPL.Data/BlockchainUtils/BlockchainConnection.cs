using Nethereum.RPC.Accounts;
using Nethereum.Web3;

namespace CRPL.Data.BlockchainUtils;

public interface IBlockchainConnection : IDisposable
{
    
}

public class BlockchainConnection : IBlockchainConnection
{
    public Web3 Web3 { get; }

    public BlockchainConnection(string chainUrl, IAccount account)
    {
        Web3 = new Web3(account, chainUrl);
    }

    public BlockchainConnection(string chainUrl)
    {
        Web3 = new Web3(chainUrl);
    }
    
    public void Dispose()
    {
    }
}