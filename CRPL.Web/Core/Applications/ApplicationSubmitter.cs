using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public static class ApplicationSubmitter
{
    public static async Task<Application> Submit(this Application submittedApplication, IServiceProvider serviceProvider)
    {
        var registrationService = serviceProvider.GetRequiredService<IRegistrationService>();
        var copyrightService = serviceProvider.GetRequiredService<ICopyrightService>();
        var accountManagementService = serviceProvider.GetRequiredService<IAccountManagementService>();
        
        switch (submittedApplication.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return await CopyrightRegistrationSubmitter((CopyrightRegistrationApplication)submittedApplication, registrationService);
            case ApplicationType.OwnershipRestructure:
                return await OwnershipRestructureSubmitter((OwnershipRestructureApplication)submittedApplication, copyrightService);
            case ApplicationType.Dispute:
                return await DisputeSubmitter((DisputeApplication)submittedApplication, copyrightService);
            case ApplicationType.DeleteAccount:
                return await DeleteAccountSubmitter((DeleteAccountApplication)submittedApplication, accountManagementService);
            case ApplicationType.WalletTransfer:
                return await WalletTransferSubmitter((WalletTransferApplication)submittedApplication, accountManagementService);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task<Application> WalletTransferSubmitter(WalletTransferApplication walletTransferApplication, IAccountManagementService accountManagementService)
    {
        walletTransferApplication.Status = ApplicationStatus.Submitted;
        
        return await accountManagementService.WalletTransfer(walletTransferApplication);
    }

    private static async Task<Application> DeleteAccountSubmitter(DeleteAccountApplication deleteAccountApplication, IAccountManagementService accountManagementService)
    {
        deleteAccountApplication.Status = ApplicationStatus.Submitted;

        return await accountManagementService.DeleteUser(deleteAccountApplication);
    }

    private static async Task<Application> CopyrightRegistrationSubmitter(CopyrightRegistrationApplication copyrightRegistrationApplication, IRegistrationService registrationService)
    {
        await registrationService.StartRegistration(copyrightRegistrationApplication);

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