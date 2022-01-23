using System.Numerics;

namespace CRPL.Data.Account;

public class UserWallet
{
    public Guid Id { get; set; }
    public string PublicAddress { get; set; }
    public long Nonce { get; set; }
    
    public List<RegisteredWork> RegisteredWorks { get; set; }
}