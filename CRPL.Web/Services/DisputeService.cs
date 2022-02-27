using System.Numerics;
using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Data.StructuredOwnership;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexTypes;

namespace CRPL.Web.Services;

public class DisputeService : IDisputeService
{
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;
    private readonly IFormsService FormsService;
    private readonly ILogger<DisputeService> Logger;

    public DisputeService(
        ILogger<DisputeService> logger,
        ApplicationContext context,
        IMapper mapper,
        IBlockchainConnection blockchainConnection,
        IContractRepository contractRepository,
        IFormsService formsService)
    {
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
        FormsService = formsService;
        Logger = logger;
    }

    public async Task<DisputeViewModel> AcceptRecourse(Guid disputeId, string message)
    {
        Logger.LogInformation("Accepting recourse for dispute {Id}", disputeId);

        var dispute = await Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == disputeId);
        if (dispute == null) throw new DisputeNotFoundException(disputeId);

        if (dispute.Status != ApplicationStatus.Submitted) throw new Exception("Dispute not submitted");

        Context.Update(dispute);

        dispute.ResolveResult.Rejected = false;
        dispute.ResolveResult.Message = message;
        dispute.ResolveResult.ResolvedStatus = ResolveStatus.NeedsOnChainAction;

        await Context.SaveChangesAsync();

        if (dispute.ExpectedRecourse == ExpectedRecourse.ChangeOfOwnership) await RestructureAndResolve(disputeId);

        return Mapper.Map<DisputeViewModel>(dispute);
    }

    public async Task<DisputeViewModel> RejectRecourse(Guid disputeId, string message)
    {
        Logger.LogInformation("Rejecting recourse for dispute {Id}", disputeId);

        var dispute = await Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == disputeId);
        if (dispute == null) throw new DisputeNotFoundException(disputeId);

        if (dispute.Status != ApplicationStatus.Submitted) throw new Exception("Dispute not submitted");

        Context.Update(dispute);

        dispute.ResolveResult.Rejected = true;
        dispute.ResolveResult.Message = message;
        dispute.ResolveResult.ResolvedStatus = ResolveStatus.Resolved;
        dispute.Status = ApplicationStatus.Complete;

        await Context.SaveChangesAsync();

        return Mapper.Map<DisputeViewModel>(dispute);
    }

    public async Task RecordPaymentAndResolve(Guid disputeId, string transaction)
    {
        Logger.LogInformation("Recording payment and resolving dispute {Id}", disputeId);

        var dispute = await Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == disputeId);
        if (dispute == null) throw new DisputeNotFoundException(disputeId);
        if (dispute.Status != ApplicationStatus.Submitted) throw new Exception("Dispute not submitted");
        if (dispute.ExpectedRecourse != ExpectedRecourse.Payment) throw new Exception("Expected recourse is not payment");

        // TODO: receipt checking doesn't work in current implementation as the transaction is probably not confirmed by this time 
        // var receipt = await BlockchainConnection.Web3().Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction);
        // if (receipt == null) throw new Exception("Transaction not found!");
        // if (receipt.Status.Value == BigInteger.Zero) throw new Exception("That payment has failed");

        Context.Update(dispute);

        dispute.ResolveResult.Transaction = transaction;
        dispute.ResolveResult.ResolvedStatus = ResolveStatus.Resolved;
        dispute.Status = ApplicationStatus.Complete;

        await Context.SaveChangesAsync();
    }

    public async Task RestructureAndResolve(Guid disputeId)
    {
        Logger.LogInformation("Creating a restructure proposal to satisfy a dispute expected recourse");

        var dispute = await Context.DisputeApplications
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.Id == disputeId);
        
        if (dispute == null) throw new DisputeNotFoundException(disputeId);
        if (dispute.AssociatedWork == null) throw new WorkNotFoundException();
        if (dispute.Status != ApplicationStatus.Submitted) throw new Exception("Dispute not submitted");
        if (dispute.ExpectedRecourse != ExpectedRecourse.ChangeOfOwnership) throw new Exception("Expected recourse is not ownership");

        Context.Update(dispute);

        dispute.ResolveResult.ResolvedStatus = ResolveStatus.Processing;
        
        var ownershipOf = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
            .OwnershipOfQueryAsync(BigInteger.Parse(dispute.AssociatedWork.RightId));

        var form = await FormsService.Update<OwnershipRestructureViewModel>(new OwnershipRestructureInputModel
        {
            ProposedStructure = new List<OwnershipStake>
            {
                new()
                {
                    Owner = dispute.AssociatedUsers.First().UserAccount.Wallet.PublicAddress,
                    Share = 100
                }
            },
            CurrentStructure = ownershipOf.ReturnValue1.Select(x => Mapper.Map<OwnershipStake>(x)).ToList(),
            WorkId = dispute.AssociatedWork.Id,
            RestructureReason = RestructureReason.Dispute,
            Origin = dispute
        });

        await FormsService.Submit<OwnershipRestructureApplication, OwnershipRestructureViewModel>(form.Id);
    }
}