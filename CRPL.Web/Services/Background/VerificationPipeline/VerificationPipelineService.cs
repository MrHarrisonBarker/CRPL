using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services.Background.VerificationPipeline;

public class VerificationPipelineService : BackgroundService
{
    private readonly ILogger<VerificationPipelineService> Logger;
    private readonly IServiceProvider ServiceProvider;
    private readonly IVerificationQueue VerificationQueue;

    public VerificationPipelineService(ILogger<VerificationPipelineService> logger, IServiceProvider serviceProvider, IVerificationQueue verificationQueue)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
        VerificationQueue = verificationQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var nextWork = await VerificationQueue.DequeueAsync(cancellationToken);

            using var scope = ServiceProvider.CreateScope();
            var worksVerificationService = scope.ServiceProvider.GetRequiredService<IWorksVerificationService>();

            try
            {
                await worksVerificationService.VerifyWork(nextWork);
            }
            catch (Exception e)
            {
                Logger.LogError(e,"There was a problem when verifying a work");
            }
        }
    }
}