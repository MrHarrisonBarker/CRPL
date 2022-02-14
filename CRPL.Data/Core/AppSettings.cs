using System.Numerics;
using CRPL.Data.BlockchainUtils;

namespace CRPL.Data;

public class AppSettings
{
    public string ConnectionString { get; set; }
    public string EncryptionKey { get; set; }

    // public Blockchains CurrentChain { get; set; }

    public List<Chain> Chains { get; set; }
    public SystemAccount SystemAccount { get; set; }
}

public class SystemAccount
{
    public string AccountId { get; set; }
    public string PrivateKey { get; set; }
}

public class Chain
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Id { get; set; }
    public BigInteger ChainIdInt() => BigInteger.Parse(Id);
}