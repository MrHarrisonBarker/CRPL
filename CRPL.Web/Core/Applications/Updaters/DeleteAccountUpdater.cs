using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;

namespace CRPL.Web.Services.Updaters;

public static class DeleteAccountUpdater
{
    private static readonly List<string> Encodables = new() { "OwnershipStakes", "Id"  };
    
    public static async Task<DeleteAccountApplication> Update(this DeleteAccountApplication application, DeleteAccountInputModel inputModel)
    {
        application.AccountId = inputModel.AccountId;
        
        application.UpdateProperties(inputModel, Encodables);
        
        return application;
    }
}