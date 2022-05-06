using CRPL.Data.Applications;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

// A Submitter class for ownership restructure applications
public static class OwnershipRestructureSubmitter
{
    // When the user submits the application propose the restructure and update status
    public static async Task<OwnershipRestructureApplication> Submit(this OwnershipRestructureApplication ownershipRestructureApplication, IServiceProvider serviceProvider)
    {
        var copyrightService = serviceProvider.GetRequiredService<ICopyrightService>();
        
        ownershipRestructureApplication = await copyrightService.ProposeRestructure(ownershipRestructureApplication);

        ownershipRestructureApplication.Status = ApplicationStatus.Submitted;
        
        return ownershipRestructureApplication;
    }
}