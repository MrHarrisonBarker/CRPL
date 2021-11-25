using Nethereum.RPC.Accounts;
using Nethereum.Web3;

namespace PIPO.Interface;

public class BlockChainConnection : IDisposable
{
    public Web3 Web3 { get; private set; }

    public BlockChainConnection(string chainUrl, IAccount account)
    {
        Web3 = new Web3(account, chainUrl)
        {
            // TransactionManager =
            // {
            //     UseLegacyAsDefault = true
            // }
        };
    }
    public void Dispose()
    {
        Web3 = null!;
    }
}