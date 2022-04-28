using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.ContractDeployment;
using CRPL.Data.Seed;
using CRPL.Data.Works;
using CRPL.Web.Core;
using CRPL.Web.Services.Background;
using CRPL.Web.Services.Background.SlientExpiry;
using CRPL.Web.Services.Background.Usage;
using CRPL.Web.Services.Background.VerificationPipeline;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.StartUp;

public static class DbExtensions
{
    public static DbPipelineBuilder AddDbPipeline(this IServiceCollection services, AppSettings settings) => new DbPipelineBuilder(services, settings);
    public static UseSeedingBuilder UseSeeding(this WebApplication app) => new UseSeedingBuilder(app);
    public static UseWakeServices WakeServices(this WebApplication app) => new UseWakeServices(app);
}

public class UseSeedingBuilder
{
    public UseSeedingBuilder(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        context.Database.EnsureCreated();
        context.SaveChanges();

        if (!context.UserAccounts.Any()) new UserAccountSeeder(context).Seed();

        var args = Environment.GetCommandLineArgs();
        Console.WriteLine(string.Join(",", args));
        if (args.Contains("--NUKE"))
        {
            Console.WriteLine("NUKE NUKE NUKE NUKE NUKE");

            var contractContext = scope.ServiceProvider.GetRequiredService<ContractContext>();
            contractContext.Database.EnsureCreated();
            contractContext.DeployedContracts.RemoveRange(contractContext.DeployedContracts);
            contractContext.SaveChanges();

            var contractRepository = app.Services.GetService<IContractRepository>();

            context.Applications.RemoveRange(context.Applications);
            context.RegisteredWorks.RemoveRange(context.RegisteredWorks);
            context.UserApplications.RemoveRange(context.UserApplications);
            context.UserWorks.RemoveRange(context.UserWorks);
            context.UserAccounts.RemoveRange(context.UserAccounts);
            context.SaveChanges();

            new UserAccountSeeder(context).Seed();
        }
    }
}

public class UseWakeServices
{
    public UseWakeServices(WebApplication app)
    {
        app.Services.GetService<IContractRepository>();
        app.Services.GetService<ICachedWorkRepository>();
        app.Services.GetService<IEventQueue>();
        app.Services.GetService<IVerificationQueue>();
        app.Services.GetService<IUsageQueue>();
        app.Services.GetService<IResonanceService>();
    }
}

public class DbPipelineBuilder
{
    public DbPipelineBuilder(IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContextPool<ContractContext>(builder =>
            builder.UseMySql(appSettings.ConnectionString, new MySqlServerVersion(ServerVersion.AutoDetect(appSettings.ConnectionString)),
                optionsBuilder => optionsBuilder.MigrationsAssembly("CRPL.Web"))
        );

        services.AddDbContextPool<ApplicationContext>(builder =>
            builder.UseMySql(appSettings.ConnectionString, new MySqlServerVersion(ServerVersion.AutoDetect(appSettings.ConnectionString)),
                optionsBuilder => optionsBuilder.MigrationsAssembly("CRPL.Web"))
        );

        services.AddSingleton<IContractRepository, ContractRepository>();
        services.AddSingleton<ICachedWorkRepository, CachedWorkRepository>();
        services.AddSingleton<IEventQueue, EventQueue>();
        services.AddSingleton<IVerificationQueue, VerificationQueue>();
        services.AddSingleton<IExpiryQueue, ExpiryQueue>();
        services.AddSingleton<IUsageQueue, UsageQueue>();
        services.AddSingleton<IResonanceService, ResonanceService>();

        services.Configure<FormOptions>(o =>
        {
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
        });
    }
}