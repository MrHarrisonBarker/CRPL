using CRPL.Contracts.Copyleft;
using CRPL.Contracts.Copyleft.ContractDefinition;
using CRPL.Contracts.Permissive;
using CRPL.Contracts.Permissive.ContractDefinition;
using CRPL.Contracts.Standard;
using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Data.BlockchainUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

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
            case CopyrightContract.Standard:
                return await StandardService.DeployContractAndWaitForReceiptAsync(connection, new StandardDeployment(), cancellationTokenSource);
            case CopyrightContract.Copyleft:
                return await CopyleftService.DeployContractAndWaitForReceiptAsync(connection, new CopyleftDeployment(), cancellationTokenSource);
            case CopyrightContract.Permissive:
                return await PermissiveService.DeployContractAndWaitForReceiptAsync(connection, new PermissiveDeployment(), cancellationTokenSource);
            default:
                throw new ArgumentOutOfRangeException(nameof(contractType), contractType, null);
        }
    }

    private readonly IServiceProvider ServiceProvider;
    private readonly AppSettings AppSettings;
    private readonly ILogger<ContractRepository> Logger;

    private Dictionary<CopyrightContract, DeployedContract> DeployedContracts;

    public ContractRepository(IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, ILogger<ContractRepository> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
        AppSettings = appSettings.Value;

        Logger.LogInformation("Starting Contract Repository");

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContractContext>();

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

    private void getContracts(ContractContext contractContext)
    {
        DeployedContracts = contractContext.DeployedContracts.ToDictionary(x => x.Type, x => x);
    }

    private async void init()
    {
        using var connection = new BlockchainConnection(AppSettings.ChainUrl, new Account(AppSettings.SystemAccount.PrivateKey, AppSettings.ChainIdInt()));

        // runs with a clear workspace to deploy all contracts needed
        foreach (CopyrightContract contractType in Enum.GetValues<CopyrightContract>())
        {
            await deployContract(contractType, connection.Web3);
        }
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