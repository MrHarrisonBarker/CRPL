using CRPL.Data;
using CRPL.Data.BlockchainUtils;
using Nethereum.Web3.Accounts;

namespace CRPL.Tests;

public class TestConstants
{
    public static string TestAccountId = "0x12890d2cce102216644c59dae5baed380d84830c";
    public static string PrivateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";
    
    public static BlockchainConnection PrivateTestConnection() => new BlockchainConnection(LawsOfNature.ChainUrl, new Account(PrivateKey, LawsOfNature.ChainId));
}