namespace CRPL.Data.Applications.ViewModels;

public abstract class ApplicationViewModel
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}