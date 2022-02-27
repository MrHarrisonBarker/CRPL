namespace CRPL.Data.Applications.DataModels;

public class DeleteAccountApplication: Application
{
    public DeleteAccountApplication() : base(ApplicationType.DeleteAccount)
    {
    }
    
    public Guid AccountId { get; set; }
}