using CRPL.Data.Account;
using CRPL.Data.Account.ViewModels;
using CRPL.Data.Applications.Core;

namespace CRPL.Data.Applications.ViewModels;

public class DisputeViewModel : ApplicationViewModel
{
    public DisputeType DisputeType { get; set; }
    public string Reason { get; set; }
    public DateTime? Spotted { get; set; }
    public int? Infractions { get; set; }
    public ExpectedRecourse? ExpectedRecourse { get; set; }
    public string? ExpectedRecourseData { get; set; }
    public string? ContactAddress { get; set; }
    public string? LinkToInfraction { get; set; }
    public ResolveResultWithUri ResolveResult { get; set; }

    public RegisteredWorkViewModel DisputedWork { get; set; }
    public UserAccountMinimalViewModel Accuser { get; set; }
}

public class DisputeViewModelWithoutAssociated : ApplicationViewModelWithoutAssociated
{
    
}