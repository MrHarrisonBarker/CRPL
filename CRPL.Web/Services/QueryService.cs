using System.Linq.Dynamic.Core;
using System.Numerics;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Core.Query;
using CRPL.Web.Services.Background.SlientExpiry;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.FunctionEncoding;

namespace CRPL.Web.Services;

public class QueryService : IQueryService
{
    private readonly ILogger<QueryService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;
    private readonly IExpiryQueue ExpiryQueue;

    public QueryService(
        ILogger<QueryService> logger,
        ApplicationContext context,
        IMapper mapper,
        IBlockchainConnection blockchainConnection,
        IContractRepository contractRepository,
        IExpiryQueue expiryQueue)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
        ExpiryQueue = expiryQueue;
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
    }

    public async Task<RegisteredWorkWithAppsViewModel> GetWork(Guid id)
    {
        var work = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .PruneApplications().FirstOrDefaultAsync(x => x.Id == id);
        return await injectFromChain(Mapper.Map<RegisteredWorkWithAppsViewModel>(work));
    }

    public async Task<List<RegisteredWorkViewModel>> GetAll(int from, int take = 100)
    {
        return await Context.RegisteredWorks
            .AsNoTracking()
            .Include(x => x.AssociatedApplication)
            .Where(x => x.Status != RegisteredWorkStatus.Created)
            .OrderBy(x => x.Created)
            .Skip(from).Take(take)
            .Select(x => Mapper.Map<RegisteredWorkViewModel>(x))
            .ToListAsync();
    }

    public Task<List<RegisteredWorkViewModel>> Search(StructuredQuery query, int from, int take = 100)
    {
        Logger.LogInformation("Searching for works {Query}", query.ToString());
        var works = Context.RegisteredWorks.AsQueryable();

        if (query.Keyword != null) works = works.Where(x => x.Title.Contains(query.Keyword));

        works = query.SortBy.HasValue ? works.OrderBy(query.SortBy.ToString()) : works.OrderBy(x => x.Created);

        if (query.WorkFilters != null)
        {
            foreach (var workFilter in query.WorkFilters.Keys)
            {
                query.WorkFilters.TryGetValue(workFilter, out var data);
                if (data == null) throw new Exception("Search filter not found!");
                if (!String.IsNullOrEmpty(data)) works = works.Apply(workFilter, data);
            }
        }
        
        return works.Skip(from).Take(take).PruneApplications().Where(x => x.Status == RegisteredWorkStatus.Registered).Select(x => Mapper.Map<RegisteredWorkViewModel>(x)).ToListAsync();
    }

    public async Task<List<DisputeViewModelWithoutAssociated>> GetAllDisputes(int @from, int take = 100)
    {
        return await Context.DisputeApplications
            .Include(x => x.AssociatedWork)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Status == ApplicationStatus.Submitted)
            .OrderBy(x => x.Created)
            .Skip(from).Take(take)
            .Select(x => Mapper.Map<DisputeViewModelWithoutAssociated>(x))
            .ToListAsync();
    }

    public async Task<List<DisputeViewModel>> GetAllOwnersDisputes(Guid id)
    {
        Logger.LogInformation("Getting {Id}'s disputes on their work", id);
        return await Context.DisputeApplications
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .Include(x => x.AssociatedWork).ThenInclude(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .Where(x => x.AssociatedWork.UserWorks.Any(u => u.UserId == id))
            .Where(x => x.Status == ApplicationStatus.Complete || x.Status == ApplicationStatus.Submitted)
            .Select(x => Mapper.Map<DisputeViewModel>(x)).ToListAsync();
    }

    public async Task<List<RegisteredWorkWithAppsViewModel>> GetUsersWorks(Guid id)
    {
        Logger.LogInformation("Getting {Id}'s works", id);
        var works = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .Where(x => x.UserWorks.Any(u => u.UserId == id))
            .PruneApplications().Select(x => Mapper.Map<RegisteredWorkWithAppsViewModel>(x)).ToListAsync();

        foreach (var registeredWork in works)
        {
            if (registeredWork.Status == RegisteredWorkStatus.Registered && registeredWork.RightId != null)
            {
                await injectFromChain(registeredWork);
            }
        }

        return works;
    }

    private async Task<RegisteredWorkWithAppsViewModel> injectFromChain(RegisteredWorkWithAppsViewModel registeredWork)
    {
        Logger.LogInformation("Injecting blockchain data into registered work {Id}", registeredWork.Id);
        var rightId = BigInteger.Parse(registeredWork.RightId);
        
        try
        {
            var ownershipOf = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .OwnershipOfQueryAsync(rightId);

            var currentVotes = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .CurrentVotesQueryAsync(rightId);

            var proposal = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .ProposalQueryAsync(rightId);

            var meta = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .CopyrightMetaQueryAsync(rightId);

            registeredWork.OwnershipStructure = ownershipOf != null ? ownershipOf.ReturnValue1 : null;
            registeredWork.CurrentVotes = currentVotes != null ? currentVotes.ReturnValue1 : null;
            registeredWork.HasProposal = proposal != null && proposal.ReturnValue1 != null ? proposal.ReturnValue1.NewStructure.Count > 0 : false;
            registeredWork.Meta = meta != null ? meta.ReturnValue1 : null;
            return registeredWork;
        }
        catch (SmartContractRevertException revertException)
        {
            if (revertException.RevertMessage == "EXPIRED")
            {
                if (registeredWork.Status != RegisteredWorkStatus.Expired)
                {
                    Logger.LogInformation("got EXPIRED, setting work to expired");
                    ExpiryQueue.QueueExpire(registeredWork.Id);
                } else Logger.LogInformation("got EXPIRED but that was expected");
            }
            else throw;
        }

        return registeredWork;
    }
}