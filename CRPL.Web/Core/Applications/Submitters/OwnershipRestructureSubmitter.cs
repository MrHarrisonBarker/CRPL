using CRPL.Data.Applications;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Submitters;

public static class OwnershipRestructureSubmitter
{
    public static async Task<OwnershipRestructureApplication> Submit(this OwnershipRestructureApplication ownershipRestructureApplication, IServiceProvider serviceProvider)
    {
        var copyrightService = serviceProvider.GetRequiredService<ICopyrightService>();
        
        ownershipRestructureApplication = await copyrightService.ProposeRestructure(ownershipRestructureApplication);

        ownershipRestructureApplication.Status = ApplicationStatus.Submitted;
        
        return ownershipRestructureApplication;
    }
}