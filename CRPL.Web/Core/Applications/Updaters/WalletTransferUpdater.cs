using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Updaters;

public static class WalletTransferUpdater
{
    public static async Task<WalletTransferApplication> Update(this WalletTransferApplication application, WalletTransferInputModel inputModel, IServiceProvider serviceProvider)
    {
        var blockchainConnection = serviceProvider.GetRequiredService<IBlockchainConnection>();
        var userService = serviceProvider.GetRequiredService<IUserService>();
        
        application.WalletAddress = inputModel.WalletAddress;

        // check if wallet exists on the blockchain
        var balance = await blockchainConnection.Web3().Eth.GetBalance.SendRequestAsync(application.WalletAddress);
        if (balance == null) throw new WalletNotFoundException();

        userService.AssignToApplication(inputModel.UserId, application.Id);

        return application;
    }
}