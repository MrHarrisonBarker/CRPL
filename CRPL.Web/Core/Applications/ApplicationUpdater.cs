using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Updaters;

namespace CRPL.Web.Services;

public static class ApplicationUpdater
{
    public static async Task<Application> UpdateApplication(this Application application, ApplicationInputModel inputModel, IServiceProvider serviceProvider)
    {
        switch (application.ApplicationType)
        {
            case ApplicationType.CopyrightRegistration:
                return await ((CopyrightRegistrationApplication)application).Update((CopyrightRegistrationInputModel)inputModel, serviceProvider);
            case ApplicationType.OwnershipRestructure:
                return await ((OwnershipRestructureApplication)application).Update((OwnershipRestructureInputModel)inputModel, serviceProvider);
            case ApplicationType.Dispute:
                return await ((DisputeApplication)application).Update((DisputeInputModel)inputModel, serviceProvider);
            case ApplicationType.DeleteAccount:
                return await ((DeleteAccountApplication)application).Update((DeleteAccountInputModel)inputModel);
            case ApplicationType.WalletTransfer:
                return await ((WalletTransferApplication)application).Update((WalletTransferInputModel)inputModel, serviceProvider);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}