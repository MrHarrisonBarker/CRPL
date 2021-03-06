using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using Nethereum.BlockchainProcessing;

namespace CRPL.Web.Services.Background;

// A background service for processing blockchain events
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

        // Register listeners and callback
        List<BlockchainProcessor> processors = new List<BlockchainProcessor>
        {
            BlockchainConnection.Web3().Processing.Logs
                .CreateProcessorForContract<RegisteredEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs
                .CreateProcessorForContract<ApprovedEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs
                .CreateProcessorForContract<ProposedRestructureEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs
                .CreateProcessorForContract<RestructuredEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs
                .CreateProcessorForContract<FailedProposalEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, log => EventQueue.QueueEvent(log))
        };

        // Start processors in a thread
        processors.ForEach(x => Task.Run(async () =>
        {
            Logger.LogInformation("processing logs from block {Block}", latestBlock);
            await x.ExecuteAsync(stoppingToken, latestBlock);
        }, stoppingToken));
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