using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Data.BlockchainUtils;
using Nethereum.ABI.FunctionEncoding.Attributes;
using RegisteredEventDTO = CRPL.Contracts.Standard.ContractDefinition.RegisteredEventDTO;

namespace CRPL.Web.Services.Background;

public class BlockchainEventListener : BackgroundService
{
    private readonly ILogger<BlockchainEventListener> Logger;
    private readonly IEventQueue EventQueue;
    private readonly IBlockchainConnection BlockchainConnection;

    public BlockchainEventListener(ILogger<BlockchainEventListener> logger, IServiceProvider serviceProvider, IEventQueue eventQueue)
    {
        Logger = logger;
        EventQueue = eventQueue;

        using var scope = serviceProvider.CreateScope();
        BlockchainConnection = scope.ServiceProvider.GetRequiredService<IBlockchainConnection>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Starting blockchain event listener");
        var latestBlock = await BlockchainConnection.Web3().Eth.Blocks.GetBlockNumber.SendRequestAsync();

        var processor = BlockchainConnection.Web3().Processing.Logs.CreateProcessor<RegisteredEventDTO>(log => EventQueue.QueueEvent(log));

        await processor.ExecuteAsync(stoppingToken, latestBlock);
    }
}