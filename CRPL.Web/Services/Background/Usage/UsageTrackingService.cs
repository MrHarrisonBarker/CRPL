using CRPL.Data.Account;
using CRPL.Data.Account.Works;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services.Background.Usage;

// A background service for processing work usages
public class UsageTrackingService : BackgroundService
{
    private readonly ILogger<UsageTrackingService> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly IUsageQueue UsageQueue;

    public UsageTrackingService(ILogger<UsageTrackingService> logger, IServiceProvider serviceProvider, IUsageQueue usageQueue)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        UsageQueue = usageQueue;
    }

    // dequeue usage and update stats
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Starting usage tracking service");

        while (!cancellationToken.IsCancellationRequested)
        {
            var workUsage = await UsageQueue.DequeueAsync(cancellationToken);
            Logger.LogInformation("Processing usage for {WorkId}", workUsage.WorkId);
            
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            var work = await context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == workUsage.WorkId, cancellationToken: cancellationToken);
            if (work == null) throw new WorkNotFoundException(workUsage.WorkId);

            context.Update(work);

            // Update based on type of usage
            switch (workUsage.UsageType)
            {
                case UsageType.Proxy:
                    work.TimesProxyUsed++;
                    work.LastProxyUse = workUsage.TimeStamp;
                    break;
                case UsageType.Ping:
                    work.TimesPinged++;
                    work.LastPing = workUsage.TimeStamp;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}