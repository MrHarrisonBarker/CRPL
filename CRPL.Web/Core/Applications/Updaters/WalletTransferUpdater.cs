using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Nethereum.Util;

namespace CRPL.Web.Services.Updaters;

public static class WalletTransferUpdater
{
    public static async Task<WalletTransferApplication> Update(this WalletTransferApplication application, WalletTransferInputModel inputModel, IServiceProvider serviceProvider)
    {
        var blockchainConnection = serviceProvider.GetRequiredService<IBlockchainConnection>();
        var userService = serviceProvider.GetRequiredService<IUserService>();
        
        application.WalletAddress = inputModel.WalletAddress;

        // check if wallet address is valid
        if (!new AddressUtil().IsValidEthereumAddressHexFormat(application.WalletAddress)) throw new WalletNotFoundException();

        userService.AssignToApplication(inputModel.UserId, application.Id);

        return application;
    }
}