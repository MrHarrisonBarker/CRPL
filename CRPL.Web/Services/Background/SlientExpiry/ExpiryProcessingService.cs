using CRPL.Data.Account;
using CRPL.Web.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services.Background.SlientExpiry;

public class ExpiryProcessingService : BackgroundService
{
    private readonly ILogger<ExpiryProcessingService> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly IExpiryQueue ExpiryQueue;

    public ExpiryProcessingService(ILogger<ExpiryProcessingService> logger, IServiceProvider serviceProvider, IExpiryQueue expiryQueue)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        ExpiryQueue = expiryQueue;
    }


    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var workId = await ExpiryQueue.DequeueAsync(cancellationToken);
            Logger.LogInformation("Processing expiry for {Id}", workId);
            
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var work = await context.RegisteredWorks.FirstOrDefaultAsync(x => x.Id == workId, cancellationToken: cancellationToken);
            if (work == null) throw new WorkNotFoundException(workId);

            context.Update(work);

            work.Status = RegisteredWorkStatus.Expired;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}