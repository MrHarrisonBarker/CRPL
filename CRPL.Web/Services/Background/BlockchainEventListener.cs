using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;

namespace CRPL.Web.Services.Background;

public class BlockchainEventListener : BackgroundService
{
    private readonly ILogger<BlockchainEventListener> Logger;
    private readonly IEventQueue EventQueue;
    private readonly IContractRepository ContractRepository;
    private readonly IBlockchainConnection BlockchainConnection;

    public BlockchainEventListener(ILogger<BlockchainEventListener> logger, IServiceProvider serviceProvider, IEventQueue eventQueue, IContractRepository contractRepository)
    {
        Logger = logger;
        EventQueue = eventQueue;
        ContractRepository = contractRepository;

        using var scope = serviceProvider.CreateScope();
        BlockchainConnection = scope.ServiceProvider.GetRequiredService<IBlockchainConnection>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Starting blockchain event listener");
        var latestBlock = await BlockchainConnection.Web3().Eth.Blocks.GetBlockNumber.SendRequestAsync();

        var processor = BlockchainConnection.Web3().Processing.Logs
            .CreateProcessorForContract<RegisteredEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log));

        var approvedProcessor = BlockchainConnection.Web3().Processing.Logs
            .CreateProcessorForContract<ApprovedEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log));
        
        var proposedProcessor = BlockchainConnection.Web3().Processing.Logs
            .CreateProcessorForContract<ProposedRestructureEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log));
        
        Task.Run(async () => await processor.ExecuteAsync(stoppingToken));
        Task.Run(async () => await proposedProcessor.ExecuteAsync(stoppingToken));
        await Task.Run(async () => await approvedProcessor.ExecuteAsync(stoppingToken));
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Stopping the blockchain event listener");
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        Logger.LogInformation("Disposing of the blockchain event listener");
        base.Dispose();
    }
}