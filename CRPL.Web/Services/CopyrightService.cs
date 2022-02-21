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
using CRPL.Web.Services.Background.SlientExpiry;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.FunctionEncoding;

namespace CRPL.Web.Services;

public class CopyrightService : ICopyrightService
{
    private readonly ILogger<CopyrightService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;
    private readonly IExpiryQueue ExpiryQueue;

    public CopyrightService(
        ILogger<CopyrightService> logger,
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

        var propose = new ProposeRestructureFunction
        {
            RightId = BigInteger.Parse(application.AssociatedWork.RightId),
            Restructured = application.ProposedStructure.Decode().Select(x => Mapper.Map<OwnershipStakeContract>(x)).ToList()
        };

        try
        {
            var transactionId = await new CRPL.Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .ProposeRestructureRequestAsync(propose);

            Context.Update(application);
            application.TransactionId = transactionId;
            application.AssociatedWork.ProposalTransactionId = transactionId;

            Logger.LogInformation("sent ownership proposal transaction at {Id}", transactionId);

            await Context.SaveChangesAsync();

            return application;
        }
        catch (SmartContractRevertException revertException)
        {
            if (revertException.RevertMessage == "EXPIRED")
            {
                if (application.AssociatedWork.Status != RegisteredWorkStatus.Expired)
                {
                    Logger.LogInformation("got EXPIRED, setting work to expired");
                    ExpiryQueue.QueueExpire(application.AssociatedWork.Id);
                }
                else Logger.LogInformation("got EXPIRED but that was expected");

                throw new WorkExpiredException(application.AssociatedWork.Id);
            }

            throw;
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
        var work = await Context.RegisteredWorks.Include(x => x.AssociatedApplication).FirstOrDefaultAsync(x => x.Id == proposalInput.WorkId);
        if (work == null) throw new WorkNotFoundException(proposalInput.WorkId);

        await sendBind(work, proposalInput.Accepted);
    }

    private async Task<string> sendBind(RegisteredWork work, bool accepted)
    {
        Logger.LogInformation("Sending proposal bind transaction for {Id} with the answer {accpted}", work.RightId, accepted);
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

            return transactionId;
        }
        catch (SmartContractRevertException revertException)
        {
            if (revertException.RevertMessage == "EXPIRED")
            {
                if (work.Status != RegisteredWorkStatus.Expired)
                {
                    Logger.LogInformation("got EXPIRED, setting work to expired");
                    ExpiryQueue.QueueExpire(work.Id);
                }
                else Logger.LogInformation("got EXPIRED but that was expected");

                throw new WorkExpiredException(work.Id);
            }

            throw;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when sending restructure bind transaction");
            throw;
        }
    }
}