using System.Linq.Dynamic.Core;
using System.Numerics;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Core.Query;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class QueryService : IQueryService
{
    private readonly ILogger<QueryService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;

    public QueryService(ILogger<QueryService> logger, ApplicationContext context, IMapper mapper, IBlockchainConnection blockchainConnection, IContractRepository contractRepository)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
    }

    public async Task<RegisteredWorkWithAppsViewModel> GetWork(Guid id)
    {
        return await injectFromChain(Mapper.Map<RegisteredWorkWithAppsViewModel>(await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id)));
    }

    public async Task<List<RegisteredWorkViewModel>> GetAll(int from, int take = 100)
    {
        return await Context.RegisteredWorks
            .Where(x => x.Status != RegisteredWorkStatus.Created)
            .OrderBy(x => x.Created)
            .Skip(from).Take(take)
            .Select(x => Mapper.Map<RegisteredWorkViewModel>(x))
            .ToListAsync();
    }

    public Task<List<RegisteredWorkViewModel>> Search(StructuredQuery query, int from, int take = 100)
    {
        var works = Context.RegisteredWorks.AsQueryable();

        if (query.Keyword != null) works = works.Where(x => x.Title.Contains(query.Keyword));
        if (query.SortBy.HasValue) works = works.OrderBy(query.SortBy.ToString());

        if (query.WorkFilters != null)
        {
            foreach (var workFilter in query.WorkFilters.Keys)
            {
                query.WorkFilters.TryGetValue(workFilter, out var data);
                if (data == null) throw new Exception("Search filter not found!");
                works = works.Apply(workFilter, data);
            }
        }
        
        return works.Skip(from).Take(take).Select(x => Mapper.Map<RegisteredWorkViewModel>(x)).ToListAsync();
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
                await injectFromChain(registeredWork);
            }
        }

        return works;
    }

    private async Task<RegisteredWorkWithAppsViewModel> injectFromChain(RegisteredWorkWithAppsViewModel registeredWork)
    {
        Logger.LogInformation("Injecting blockchain data into registered work {Id}", registeredWork.Id);
        var rightId = BigInteger.Parse(registeredWork.RightId);

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
        registeredWork.HasProposal = proposal != null ? (proposal.ReturnValue1.NewStructure.Count > 0) : false;
        registeredWork.Meta = meta != null ? meta.ReturnValue1 : null;
        return registeredWork;
    }
}