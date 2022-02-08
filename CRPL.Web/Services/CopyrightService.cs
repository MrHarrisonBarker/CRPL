using System.Numerics;
using AutoMapper;
using CRPL.Contracts.Standard;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nethereum.Contracts;

namespace CRPL.Web.Services;

public class CopyrightService : ICopyrightService
{
    private readonly ILogger<CopyrightService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;

    public CopyrightService(ILogger<CopyrightService> logger, ApplicationContext context, IMapper mapper, IBlockchainConnection blockchainConnection, IContractRepository contractRepository)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
    }

    public async Task<List<RegisteredWorkWithAppsViewModel>> GetUsersWorks(Guid id)
    {
        Logger.LogInformation("Getting {Id}'s works", id);
        var works = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .Where(x => x.UserWorks.Any(u => u.UserId == id)).Select(x => Mapper.Map<RegisteredWorkWithAppsViewModel>(x)).ToListAsync();

        foreach (var registeredWork in works)
        {
            if (registeredWork.Status == RegisteredWorkStatus.Registered && registeredWork.RightId != null)
            {
                Logger.LogInformation("{Id} has a registered work, getting info from the blockchain", id);
                var res =
                    await new StandardService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Standard).Address)
                        .OwnershipOfQueryAsync(BigInteger.Parse(registeredWork.RightId));
                if (res != null) registeredWork.OwnershipStructure = res.ReturnValue1;
            }
        }

        return works;
    }
}