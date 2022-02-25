using System.Numerics;
using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.Core;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
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
    private readonly ILogger<DisputeService> Logger;

    public DisputeService(ILogger<DisputeService> logger, ApplicationContext context, IMapper mapper, IBlockchainConnection blockchainConnection)
    {
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        Logger = logger;
    }

    public async Task<DisputeViewModel> AcceptRecourse(Guid disputeId, string message)
    {
        var dispute = await Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == disputeId);
        if (dispute == null) throw new DisputeNotFoundException(disputeId);

        if (dispute.Status != ApplicationStatus.Submitted) throw new Exception("Dispute not submitted");
        
        Context.Update(dispute);

        dispute.ResolveResult.Rejected = false;
        dispute.ResolveResult.Message = message;
        dispute.ResolveResult.ResolvedStatus = ResolveStatus.NeedsOnChainAction;
        
        await Context.SaveChangesAsync();

        return Mapper.Map<DisputeViewModel>(dispute);
    }

    public async Task<DisputeViewModel> RejectRecourse(Guid disputeId, string message)
    {
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
        var dispute = await Context.DisputeApplications.FirstOrDefaultAsync(x => x.Id == disputeId);
        if (dispute == null) throw new DisputeNotFoundException(disputeId);

        if (dispute.Status != ApplicationStatus.Submitted) throw new Exception("Dispute not submitted");

        var receipt = await BlockchainConnection.Web3().Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transaction);

        if (receipt.Status.Value == BigInteger.Zero) throw new Exception("That payment has failed");
        
        Context.Update(dispute);
        
        dispute.ResolveResult.Transaction = transaction;
        dispute.ResolveResult.ResolvedStatus = ResolveStatus.Resolved;

        await Context.SaveChangesAsync();
    }
}