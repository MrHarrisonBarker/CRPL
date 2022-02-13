using System.Numerics;
using AutoMapper;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Data.Proposal;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<RegisteredWork> GetWork(Guid id)
    {
        return (await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .FirstOrDefaultAsync(x => x.Id == id))!;
    }

    public async Task AttachWorkToApplicationAndCheckValid(Guid id, Application application)
    {
        var work = await Context.RegisteredWorks.Include(x => x.AssociatedApplication).FirstOrDefaultAsync(x => x.Id == id);

        if (work == null) throw new WorkNotFoundException(id);
        if (work.Registered == null) throw new Exception("The work is not registered!");
        if (work.Status != RegisteredWorkStatus.Registered) throw new Exception("The work is not registered!");

        if (work.AssociatedApplication.Any(x => x.Id == application.Id)) return;

        Context.RegisteredWorks.Update(work);

        work.AssociatedApplication.Add(application);
    }

    public async Task<OwnershipRestructureApplication> ProposeRestructure(OwnershipRestructureApplication application)
    {
        if (application.AssociatedWork == null) throw new WorkNotFoundException();

        var handler = BlockchainConnection.Web3().Eth.GetContractTransactionHandler<ProposeRestructureFunction>();
        var propose = new ProposeRestructureFunction()
        {
            RightId = BigInteger.Parse(application.AssociatedWork.RightId),
            Restructured = application.ProposedStructure.Decode().Select(x => Mapper.Map<OwnershipStakeContract>(x)).ToList()
        };

        var estimate = await handler.EstimateGasAsync(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, propose);

        try
        {
            var transactionId = await handler.SendRequestAsync(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, propose);

            Context.Update(application);
            application.TransactionId = transactionId;
            application.AssociatedWork.ProposalTransactionId = transactionId;

            Logger.LogInformation("sent ownership proposal transaction at {Id}", transactionId);

            await Context.SaveChangesAsync();

            return application;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when sending restructure proposal transaction");
            throw;
        }
    }

    public async Task BindProposal(BindProposalInput proposalInput)
    {
        Logger.LogInformation("Binding proposal using application {Id}", proposalInput.ApplicationId);
        var application = await Context.Applications.Include(x => x.AssociatedWork).FirstOrDefaultAsync(x => x.Id == proposalInput.ApplicationId);
        if (application == null) throw new ApplicationNotFoundException(proposalInput.ApplicationId);
        if (application.AssociatedWork == null) throw new WorkNotFoundException();

        await sendBind(application.AssociatedWork, proposalInput.Accepted);
    }

    public async Task BindProposal(BindProposalWorkInput proposalInput)
    {
        Logger.LogInformation("Binding proposal using work {Id}", proposalInput.WorkId);
        var work = await Context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == proposalInput.WorkId);
        if (work == null) throw new WorkNotFoundException(proposalInput.WorkId);

        await sendBind(work, proposalInput.Accepted);
    }

    private async Task sendBind(RegisteredWork work, bool accepted)
    {
        Logger.LogInformation("Sending proposal bind transaction for {Id} with the answer {accpted}", work.RightId, accepted);
        var handler = BlockchainConnection.Web3().Eth.GetContractTransactionHandler<BindRestructureFunction>();
        var bind = new BindRestructureFunction()
        {
            RightId = BigInteger.Parse(work.RightId),
            Accepted = accepted
        };

        try
        {
            var transactionId = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .BindRestructureRequestAsync(bind);

            Logger.LogInformation("sent restructure bind transaction at {Id}", transactionId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when sending restructure bind transaction");
            throw;
        }
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
            }
        }

        return works;
    }
}