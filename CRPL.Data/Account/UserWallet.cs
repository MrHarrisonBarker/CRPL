using Microsoft.EntityFrameworkCore;

namespace CRPL.Data.Account;

[Owned]
public class UserWallet
{
    public string PublicAddress { get; set; }
    public string Nonce { get; set; }
}