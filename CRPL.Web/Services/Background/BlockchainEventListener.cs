using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using Nethereum.BlockchainProcessing;

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

        List<BlockchainProcessor> processors = new List<BlockchainProcessor>()
        {
            BlockchainConnection.Web3().Processing.Logs.CreateProcessorForContract<RegisteredEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs.CreateProcessorForContract<ApprovedEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs.CreateProcessorForContract<ProposedRestructureEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs.CreateProcessorForContract<RestructuredEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log)),
            BlockchainConnection.Web3().Processing.Logs.CreateProcessorForContract<FailedProposalEventDTO>(ContractRepository.DeployedContract(CopyrightContract.Standard).Address, log => EventQueue.QueueEvent(log))
        };
        
        processors.ForEach(x => Task.Run(async () => await x.ExecuteAsync(stoppingToken), stoppingToken));
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