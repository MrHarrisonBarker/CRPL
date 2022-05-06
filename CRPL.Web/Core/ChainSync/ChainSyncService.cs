using CRPL.Data.Account;
using CRPL.Web.Core.ChainSync.Synchronisers;

namespace CRPL.Web.Core.ChainSync;

// Service for regularly checking the state of the database compared with the blokchain
public class ChainSyncService : IHostedService
{
    private readonly ILogger<ChainSyncService> Logger;
    private readonly IServiceProvider ServiceProvider;

    private int PageWidth = 100;

    private Timer CronTimer;

    public ChainSyncService(ILogger<ChainSyncService> logger, IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting chain sync service");

        // Daily sync cron job
        CronTimer = new Timer(
            Sync,
            null,
            TimeSpan.Zero,
            TimeSpan.FromHours(24)
        );

        return Task.CompletedTask;
    }

    private async void Sync(object? state)
    {
        using var scope = ServiceProvider.CreateScope();
        
        var synchronisers = scope.ServiceProvider.GetServices<ISynchroniser>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        // Calculate number of pages to process
        var numberOfPages = !context.RegisteredWorks.Any() ? 0 : Math.Max(context.RegisteredWorks.Count() / PageWidth + (context.RegisteredWorks.Count() % PageWidth > 0 ? 1 : 0), 1);
        Logger.LogInformation("Syncing total of {Num} pages", numberOfPages);

        // Synchronise copyrights in batches/pages to reduce collisions with realtime operations
        for (int page = 0; page < numberOfPages; page++)
        {
            Logger.LogInformation("Processing next chain sync batch {Page} - {End}", page, page + PageWidth);

            foreach (var synchroniser in synchronisers)
            {
                await synchroniser.SynchroniseBatch(page, PageWidth);
            }
            page++;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        CronTimer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}