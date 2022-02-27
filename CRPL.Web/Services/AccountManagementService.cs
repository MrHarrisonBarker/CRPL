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
    
    public async Task<Application> DeleteUser(DeleteAccountApplication deleteAccountApplication)
    {
        Logger.LogInformation("Deleting user! {Id}", deleteAccountApplication.AccountId);
        
        var user = await Context.UserAccounts
            .Include(x => x.UserWorks).ThenInclude(x => x.RegisteredWork)
            .FirstOrDefaultAsync(x => x.Id == deleteAccountApplication.AccountId);
        if (user == null) throw new UserNotFoundException(deleteAccountApplication.AccountId);
        
        foreach (var userWork in user.UserWorks)
        {
            var ownershipOf = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .OwnershipOfQueryAsync(BigInteger.Parse(userWork.RegisteredWork.RightId));

            if (ownershipOf.ReturnValue1.Count == 1)
            {
                Logger.LogInformation("Single owner copyright so burning!");
                // TODO: burn copyright
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

        Context.UserAccounts.Remove(user);

        await Context.SaveChangesAsync();

        return deleteAccountApplication;
    }
}