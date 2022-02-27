namespace CRPL.Data.Applications.DataModels;

public class WalletTransferApplication: Application
{
    public WalletTransferApplication() : base(ApplicationType.WalletTransfer)
    {
    }
    
    public string WalletAddress { get; set; }
}