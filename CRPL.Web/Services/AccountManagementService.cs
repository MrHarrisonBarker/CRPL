using System.Numerics;
using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Data.StructuredOwnership;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class AccountManagementService : IAccountManagementService
{
    private readonly ILogger<AccountManagementService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;
    private readonly IFormsService FormsService;
    private readonly ICopyrightService CopyrightService;

    public AccountManagementService(
        ILogger<AccountManagementService> logger,
        ApplicationContext context,
        IMapper mapper,
        IBlockchainConnection blockchainConnection,
        IContractRepository contractRepository,
        IFormsService formsService)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
        FormsService = formsService;
    }    
    
    public async Task<DeleteAccountApplication> DeleteUser(DeleteAccountApplication deleteAccountApplication)
    {
        Logger.LogInformation("Deleting user! {Id}", deleteAccountApplication.AccountId);
        
        var user = await Context.UserAccounts
            .Include(x => x.UserWorks).ThenInclude(x => x.RegisteredWork).ThenInclude(x => x.AssociatedApplication)
            .Include(x => x.Applications).ThenInclude(x => x.Application)
            .FirstOrDefaultAsync(x => x.Id == deleteAccountApplication.AccountId);
        
        if (user == null) throw new UserNotFoundException(deleteAccountApplication.AccountId);
        
        foreach (var userWork in user.UserWorks)
        {
            if (userWork.RegisteredWork.Status == RegisteredWorkStatus.Registered)
            {
                var ownershipOf = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                    .OwnershipOfQueryAsync(BigInteger.Parse(userWork.RegisteredWork.RightId));

                if (ownershipOf.ReturnValue1.Count == 1)
                {
                    Logger.LogInformation("Single owner copyright so burning!"); 
                    // TODO: burn copyright
                    Context.Applications.RemoveRange(userWork.RegisteredWork.AssociatedApplication);
                    Context.RegisteredWorks.Remove(userWork.RegisteredWork);
                }
                else
                {
                    Logger.LogInformation("Multi ownership copyright so creating proposal");
                
                    var application = await FormsService.Update<OwnershipRestructureViewModel>(new OwnershipRestructureInputModel
                    {
                        Origin = deleteAccountApplication,
                        CurrentStructure = ownershipOf.ReturnValue1.Select(x => Mapper.Map<OwnershipStake>(x)).ToList(),
                        ProposedStructure = ownershipOf.ReturnValue1
                            .Where(x => !string.Equals(x.Owner, user.Wallet.PublicAddress, StringComparison.OrdinalIgnoreCase))
                            .Select(x => Mapper.Map<OwnershipStake>(x)).ToList(),
                        RestructureReason = RestructureReason.DeleteAccount,
                        WorkId = userWork.WorkId
                    });
            
                    await FormsService.Submit<OwnershipRestructureApplication, OwnershipRestructureViewModel>(application.Id);
                }
            }
            else
            {
                Logger.LogInformation("Removing non-registered work");
                Context.Applications.RemoveRange(userWork.RegisteredWork.AssociatedApplication);
                Context.RegisteredWorks.Remove(userWork.RegisteredWork);
            }
        }

        Context.Applications.RemoveRange(user.Applications.Select(x => x.Application));
        Context.UserApplications.RemoveRange(user.Applications);
        Context.UserAccounts.Remove(user);

        await Context.SaveChangesAsync();

        return deleteAccountApplication;
    }

    public async Task<WalletTransferApplication> WalletTransfer(WalletTransferApplication walletTransferApplication)
    {
        Logger.LogInformation("Transferring wallet to {Address}", walletTransferApplication.WalletAddress);

        var user = await Context.UserAccounts
            .Include(x => x.UserWorks).ThenInclude(x => x.RegisteredWork)
            .FirstOrDefaultAsync(x => x.Id == walletTransferApplication.AssociatedUsers[0].UserId);
        
        if (user == null) throw new UserNotFoundException(walletTransferApplication.AssociatedUsers[0].UserId);

        // Transfer all copyrights to new wallet
        foreach (var userWork in user.UserWorks)
        {
            if (userWork.RegisteredWork.Status == RegisteredWorkStatus.Registered)
            {
                var ownershipOf = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                    .OwnershipOfQueryAsync(BigInteger.Parse(userWork.RegisteredWork.RightId));

                Logger.LogInformation("Transferring copyrights to new wallet");
                
                // Creating restructure applications for all works

                var restructure = new OwnershipRestructureInputModel
                {
                    Origin = walletTransferApplication,
                    CurrentStructure = ownershipOf.ReturnValue1.Select(x => Mapper.Map<OwnershipStake>(x)).ToList(),
                    ProposedStructure = ownershipOf.ReturnValue1.Select(x =>
                    {
                        if (x.Owner.Equals(user.Wallet.PublicAddress, StringComparison.OrdinalIgnoreCase))
                        {
                            x.Owner = walletTransferApplication.WalletAddress;
                        }

                        return Mapper.Map<OwnershipStake>(x);
                    }).ToList(),
                    RestructureReason = RestructureReason.TransferWallet,
                    WorkId = userWork.WorkId
                };
                
                var application = await FormsService.Update<OwnershipRestructureViewModel>(restructure);
            
                await FormsService.Submit<OwnershipRestructureApplication, OwnershipRestructureViewModel>(application.Id);
                
            }
        }

        return walletTransferApplication;
    }
}