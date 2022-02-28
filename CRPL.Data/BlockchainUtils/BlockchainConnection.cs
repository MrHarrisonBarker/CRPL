using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Web3;

namespace CRPL.Data.BlockchainUtils;

public interface IBlockchainConnection : IDisposable
{
    public Web3 Web3();
}

public class BlockchainConnection : IBlockchainConnection
{
    private readonly ILogger<BlockchainConnection> Logger;
    private readonly AppSettings AppSettings;
    private Web3 _Web3;
    private readonly Nethereum.Web3.Accounts.Account Account;

    public BlockchainConnection(ILogger<BlockchainConnection> logger, IOptions<AppSettings> appSettings)
    {
        Logger = logger;
        AppSettings = appSettings.Value;

        var currentChain = AppSettings.Chains.FirstOrDefault(x => x.Name == Environment.GetEnvironmentVariable("CURRENT_CHAIN"));

        if (currentChain == null) throw new Exception("No current chain found!");

        Account = new Nethereum.Web3.Accounts.Account(currentChain.SystemAccount.PrivateKey, currentChain.ChainIdInt());

        _Web3 = new Web3(Account, currentChain.Url);
    }

    public Web3 Web3() => _Web3;

    public void Dispose()
    {
    }
}