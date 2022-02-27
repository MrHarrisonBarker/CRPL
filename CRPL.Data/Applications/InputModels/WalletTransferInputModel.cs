namespace CRPL.Data.Applications.InputModels;

public class WalletTransferInputModel : ApplicationInputModel
{
    public Guid UserId { get; set; }
    public string WalletAddress { get; set; }
}