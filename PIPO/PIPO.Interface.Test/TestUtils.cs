using Nethereum.Web3.Accounts;

namespace PIPO.Interface.Test;

public static class TestUtils   
{
    public static BlockChainConnection PrivateTestConnection() => new BlockChainConnection(LawsOfNature.ChainUrl, new Account(LawsOfNature.PrivateKey, LawsOfNature.ChainId));
    public static BlockChainConnection RopstenTestConnection() => new BlockChainConnection(LawsOfNature.TestNetUrl);
}