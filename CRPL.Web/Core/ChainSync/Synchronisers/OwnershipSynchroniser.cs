using System.Numerics;
using CRPL.Contracts.Copyright;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Core.ChainSync.Synchronisers;

// TODO: this synchroniser will check if the ownership structure on the blockchain matched what the system has and make changes if needed
public class OwnershipSynchroniser : ISynchroniser
{
    private readonly ILogger<OwnershipSynchroniser> Logger;
    private readonly ApplicationContext Context;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;

    public OwnershipSynchroniser(
        ILogger<OwnershipSynchroniser> logger,
        ApplicationContext context,
        IBlockchainConnection blockchainConnection,
        IContractRepository contractRepository)
    {
        Logger = logger;
        Context = context;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
    }

    public Task SynchroniseBatch()
    {
        throw new NotImplementedException();
    }

    public async Task SynchroniseOne(Guid id)
    {
        Logger.LogInformation("Synchronising ownership of work {Id}", id);
        
        var work = await Context.RegisteredWorks
            .Include(x => x.UserWorks).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (work == null) throw new WorkNotFoundException();

        var ownershipOf = (await new CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
            .OwnershipOfQueryAsync(BigInteger.Parse(work.RightId))).ReturnValue1;

        if (ownershipOf == null) throw new Exception($"The ownership of {work.RightId} cannot be found on the blockchain");

        var ownerships = ownershipOf.Select(x => x.Owner.ToLower()).ToList();

        if (ownerships.Count != work.UserWorks.Count || !work.UserWorks.All(x => ownerships.Contains(x.UserAccount.Wallet.PublicAddress.ToLower())))
        {
            Logger.LogInformation("The ownership differs from the blockchain!");
            
            Context.Update(work);
            
            work.UserWorks.Clear();
            ownerships.ForEach(async owner =>
            {
                var user = await Context.UserAccounts.FirstOrDefaultAsync(x => x.Wallet.PublicAddress.ToLower() == owner.ToLower());
                if (user == null) throw new UserNotFoundException(owner);
                work.UserWorks.Add(new UserWork()
                {
                    UserAccount = user
                });
            });

            await Context.SaveChangesAsync();
            
            return;
        }
        
        Logger.LogInformation("The ownership is in sync with the blockchain");
    }
}