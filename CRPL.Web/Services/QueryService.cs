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

// A service for querying registered works and disputes
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
    }

    // Get a specific work as a view model
    public async Task<RegisteredWorkWithAppsViewModel> GetWork(Guid id)
    {
        var work = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .PruneApplications().FirstOrDefaultAsync(x => x.Id == id);
        
        // Get data from the blockchain and insert in model
        return await injectFromChain(Mapper.Map<RegisteredWorkWithAppsViewModel>(work));
    }

    // Get all the registered works with pagination
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

    // Search registered works using a structured query with pagination
    public Task<List<RegisteredWorkViewModel>> Search(StructuredQuery query, int from, int take = 100)
    {
        Logger.LogInformation("Searching for works {Query}", query.ToString());
        var works = Context.RegisteredWorks.AsQueryable();

        // If there is a keyword search works 
        if (query.Keyword != null) works = works.Where(x => x.Title.Contains(query.Keyword));

        // Set ordering based on query
        works = query.SortBy.HasValue ? works.OrderBy(query.SortBy.ToString()) : works.OrderBy(x => x.Created);

        // If there is a filter apply filters
        if (query.WorkFilters != null)
        {
            foreach (var workFilter in query.WorkFilters.Keys)
            {
                query.WorkFilters.TryGetValue(workFilter, out var data);
                if (data == null) throw new Exception("Search filter not found!");
                
                // Apply the filter to the works
                if (!String.IsNullOrEmpty(data)) works = works.Apply(workFilter, data);
            }
        }
        
        // return list of view models
        return works.Skip(from).Take(take).PruneApplications().Where(x => x.Status == RegisteredWorkStatus.Registered).Select(x => Mapper.Map<RegisteredWorkViewModel>(x)).ToListAsync();
    }

    // Get all disputes with pagination
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

    // Get all disputes with a relationship to a user
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

    // Get all registered works with a relationship to a user
    public async Task<List<RegisteredWorkWithAppsViewModel>> GetUsersWorks(Guid id)
    {
        Logger.LogInformation("Getting {Id}'s works", id);
        var works = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .Where(x => x.UserWorks.Any(u => u.UserId == id))
            .PruneApplications().Select(x => Mapper.Map<RegisteredWorkWithAppsViewModel>(x)).ToListAsync();

        // If the work has been registered then inject data from the blockchain
        foreach (var registeredWork in works)
        {
            if (registeredWork.Status == RegisteredWorkStatus.Registered && registeredWork.RightId != null)
            {
                await injectFromChain(registeredWork);
            }
        }

        return works;
    }

    // Querying data from the smart contract for a specific registered work and update data model
    private async Task<RegisteredWorkWithAppsViewModel> injectFromChain(RegisteredWorkWithAppsViewModel registeredWork)
    {
        if (registeredWork.RightId == null) return registeredWork;
        
        Logger.LogInformation("Injecting blockchain data into registered work {Id}", registeredWork.Id);
        var rightId = BigInteger.Parse(registeredWork.RightId);

        // Query all available data
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

            // Insert in data model
            registeredWork.OwnershipStructure = ownershipOf != null ? ownershipOf.ReturnValue1 : null;
            registeredWork.CurrentVotes = currentVotes != null ? currentVotes.ReturnValue1 : null;
            registeredWork.HasProposal = proposal != null && proposal.ReturnValue1 != null ? proposal.ReturnValue1.NewStructure.Count > 0 : false;
            registeredWork.Meta = meta != null ? meta.ReturnValue1 : null;
            return registeredWork;
        }
        // Catch and handle expired copyrights
        catch (SmartContractRevertException revertException)
        {
            if (revertException.RevertMessage == "EXPIRED")
            {
                if (registeredWork.Status != RegisteredWorkStatus.Expired)
                {
                    Logger.LogInformation("got EXPIRED, setting work to expired");
                    ExpiryQueue.QueueExpire(registeredWork.Id);
                }
                else Logger.LogInformation("got EXPIRED but that was expected");
            }
            else throw;
        }

        return registeredWork;
    }
}