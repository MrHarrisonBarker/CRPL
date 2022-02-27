namespace CRPL.Web.Exceptions;

public class WalletNotFoundException : Exception
{
    public WalletNotFoundException() : base("Wallet cant be found on the chain")
    {
    }
}