using CRPL.Contracts.Structs;

namespace CRPL.Data.Applications.ViewModels;

public class CopyrightRegistrationApplication : Application
{
    public CopyrightRegistrationApplication() : base(ApplicationType.CopyrightRegistration)
    {
    }

    public string Title { get; set; }
    public byte[] WorkHash { get; set; }
    public string WorkUri { get; set; }
    public string Legal { get; set; }
    public WorkType WorkType { get; set; }
    public int YearsExpire { get; set; }
    public string OwnershipStakes { get; set; }
    public Protections Protections { get; set; }
}