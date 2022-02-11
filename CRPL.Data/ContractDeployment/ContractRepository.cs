using CRPL.Contracts.Copyright;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.BlockchainUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace CRPL.Data.ContractDeployment;

public interface IContractRepository
{
    public DeployedContract DeployedContract(CopyrightContract contract);
}

public class ContractRepository : IContractRepository
{
    private async Task<TransactionReceipt> deploymentMessage(CopyrightContract contractType, Web3 connection, CancellationTokenSource cancellationTokenSource = null)
    {
        switch (contractType)
        {
            case CopyrightContract.Copyright:
                return await CopyrightService.DeployContractAndWaitForReceiptAsync(connection, new CopyrightDeployment(), cancellationTokenSource);
            default:
                throw new ArgumentOutOfRangeException(nameof(contractType), contractType, null);
        }
    }

    private readonly IServiceProvider ServiceProvider;
    private readonly AppSettings AppSettings;
    private readonly ILogger<ContractRepository> Logger;
    private readonly IBlockchainConnection BlockchainConnection;

    private Dictionary<CopyrightContract, DeployedContract> DeployedContracts;

    public ContractRepository(IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, ILogger<ContractRepository> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
        AppSettings = appSettings.Value;

        Logger.LogInformation("Starting Contract Repository");

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContractContext>();

        BlockchainConnection = scope.ServiceProvider.GetRequiredService<IBlockchainConnection>();

        getContracts(context);

        // If no contracts are deployed then init and deploy all contracts
        if (!context.DeployedContracts.Any())
        {
            Logger.LogInformation("Not contracts found! deploying all");

            init();
        }
    }

    public DeployedContract DeployedContract(CopyrightContract contract)
    {
        if (!DeployedContracts.ContainsKey(contract)) throw new Exception("That contract doesn't exist");

        return DeployedContracts[contract];
    }

    private async void getContracts(ContractContext contractContext)
    {
        DeployedContracts = contractContext.DeployedContracts.ToDictionary(x => x.Type, x => x);
        
        // check if contracts can be found on the blockchain
        foreach (var deployedContract in DeployedContracts.Values)
        {

            var result = await BlockchainConnection.Web3().Eth.GetCode.SendRequestAsync(deployedContract.Address);

            if (result == null) throw new Exception("A saved contract doesn't exist on the blockchain");
        }
    }

    private async void init()
    {
        // runs with a clear workspace to deploy all contracts needed
        // foreach (CopyrightContract contractType in Enum.GetValues<CopyrightContract>())
        // {
            await deployContract(CopyrightContract.Copyright, BlockchainConnection.Web3());
        // }
    }

    private async Task deployContract(CopyrightContract contractType, Web3 web3)
    {
        var receipt = await deploymentMessage(contractType, web3);

        if (receipt.HasErrors()!.Value) throw new Exception($"There was an error when deploying a contract: {receipt.Logs}");

        // replaces old contract

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContractContext>();

        // context.DeployedContracts.Remove(context.DeployedContracts.First(x => x.Type == contractType));

        await context.DeployedContracts.AddAsync(new DeployedContract()
        {
            Address = receipt.ContractAddress,
            Deployed = DateTime.Now,
            Type = contractType
        });

        await context.SaveChangesAsync();
    }
}