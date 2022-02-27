using System.Numerics;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public static class ApplicationSubmitter
{
    public static async Task<Application> Submit(this Application submittedApplication, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var registrationService = scope.ServiceProvider.GetRequiredService<IRegistrationService>();
        var copyrightService = scope.ServiceProvider.GetRequiredService<ICopyrightService>();
        var accountManagementService = scope.ServiceProvider.GetRequiredService<IAccountManagementService>();
        
        switch (submittedApplication.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return CopyrightRegistrationSubmitter((CopyrightRegistrationApplication)submittedApplication, registrationService);
            case ApplicationType.OwnershipRestructure:
                return await OwnershipRestructureSubmitter((OwnershipRestructureApplication)submittedApplication, copyrightService);
            case ApplicationType.Dispute:
                return await DisputeSubmitter((DisputeApplication)submittedApplication, copyrightService);
            case ApplicationType.DeleteAccount:
                return await DeleteAccountSubmitter((DeleteAccountApplication)submittedApplication, accountManagementService);
            case ApplicationType.WalletTransfer:
            default:
                throw new ArgumentOutOfRangeException();
        }

        return submittedApplication;
    }

    private static async Task<Application> DeleteAccountSubmitter(DeleteAccountApplication deleteAccountApplication, IAccountManagementService accountManagementService)
    {
        deleteAccountApplication.Status = ApplicationStatus.Submitted;

        return await accountManagementService.DeleteUser(deleteAccountApplication);
    }

    private static Application CopyrightRegistrationSubmitter(CopyrightRegistrationApplication copyrightRegistrationApplication, IRegistrationService registrationService)
    {
        registrationService.StartRegistration(copyrightRegistrationApplication);

        copyrightRegistrationApplication.Status = ApplicationStatus.Submitted;
        
        return copyrightRegistrationApplication;
    }
    
    private static async Task<Application> OwnershipRestructureSubmitter(OwnershipRestructureApplication ownershipRestructureApplication, ICopyrightService copyrightService)
    {
        ownershipRestructureApplication = await copyrightService.ProposeRestructure(ownershipRestructureApplication);

        ownershipRestructureApplication.Status = ApplicationStatus.Submitted;
        
        return ownershipRestructureApplication;
    }

    private static async Task<Application> DisputeSubmitter(DisputeApplication disputeApplication, ICopyrightService copyrightService)
    {
        disputeApplication.Status = ApplicationStatus.Submitted;
        
        return disputeApplication;
    }
}