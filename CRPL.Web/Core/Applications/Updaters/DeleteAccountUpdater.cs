using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;

namespace CRPL.Web.Services.Updaters;

// An updater class for delete account applications
public static class DeleteAccountUpdater
{
    private static readonly List<string> Encodables = new() { "OwnershipStakes", "Id"  };
    
    // Update properties on the data model based on input
    public static async Task<DeleteAccountApplication> Update(this DeleteAccountApplication application, DeleteAccountInputModel inputModel)
    {
        application.AccountId = inputModel.AccountId;
        
        // Encodables are ignored properties 
        application.UpdateProperties(inputModel, Encodables);
        
        return application;
    }
}