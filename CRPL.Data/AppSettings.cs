using System.Numerics;

namespace CRPL.Data;

public class AppSettings
{
    public string ConnectionString { get; set; }
    public string ChainUrl { get; set; }
    public string ChainId { get; set; }
    
    public BigInteger ChainIdInt() => BigInteger.Parse(ChainId);

    public SystemAccount SystemAccount { get; set; }
}

public class SystemAccount
{
    public string AccountId { get; set; }
    public string PrivateKey { get; set; }
}
