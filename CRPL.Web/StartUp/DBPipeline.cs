using System.Text;
using CRPL.Contracts.Structs;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.ContractDeployment;
using CRPL.Data.Seed;
using CRPL.Data.StructuredOwnership;
using CRPL.Data.Works;
using CRPL.Web.Services.Background;
using CRPL.Web.Services.Background.SlientExpiry;
using CRPL.Web.Services.Background.VerificationPipeline;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.StartUp;

public static class DbExtensions
{
    public static DbPipelineBuilder AddDbPipeline(this IServiceCollection services, AppSettings settings) => new DbPipelineBuilder(services, settings);
    public static UseSeedingBuilder UseSeeding(this WebApplication app) => new UseSeedingBuilder(app);
    public static UseWakeServices WakeServices(this WebApplication app) => new UseWakeServices(app);
    public static UseCopyrightSeedingBuilder UseCopyrightSeeding(this WebApplication app) => new UseCopyrightSeedingBuilder(app);
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

public class UseCopyrightSeedingBuilder
{
    public UseCopyrightSeedingBuilder(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        if (context.UserAccounts.FirstOrDefault(x => x.Id == new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C")) != null)
        {
            var formsService = scope.ServiceProvider.GetRequiredService<IFormsService>();

            Task.Run(async () =>
            {
                var application = await formsService.Update<CopyrightRegistrationViewModel>(new CopyrightRegistrationInputModel()
                {
                    Legal = "LEGAL META",
                    Title = "Hello world",
                    Protections = new Protections()
                    {
                        Authorship = true,
                        CommercialAdaptation = true,
                        CommercialDistribution = true,
                        CommercialPerformance = true,
                        CommercialReproduction = true,
                        NonCommercialAdaptation = true,
                        NonCommercialDistribution = true,
                        NonCommercialPerformance = true,
                        NonCommercialReproduction = true,
                        ReviewOrCrit = true
                    },
                    WorkHash = Encoding.UTF8.GetBytes(
                        "c87c3b1d39b5e23a10ab3fe27f14ec54bf6b7043c27299acfff56bf503cb9c10abb51fe5944936a3dc4ea1d0fb29882ea0078457a316c34e44587bbe08ea64f81e20839079f493737664a770b33a1d915fe7e9278862ac2cfa5f93143e38377996949d8d9fcb795fff463842a0b5c2e90f4faf03c876877ae63a9f48df60d816f4add8a48d802019ae39a2b394634c23803ce99dbee3ae8f52277bee7719cfcaceafd41982a6015eed3f65e7657819f3c785ce9a554537f71d0fb9c9eceae2cea0c6aca39b39933b949ab2c7a6906f37d09bc8c893b6f10d0a2e9328da8149d043b5a5397c8257d41c6ffaee6e5ddfe0c4a728e31dc804d34a7c2caae8ff431940352f4fa726f9eb5f8e26dbc218613c36cf1b9d00b71d3559f6c3a55a1c1d43e0d35a5c2c6b0f2ea224cb0896649fcc70923696210b9b8f58e04c7e802e6d7e"),
                    WorkType = WorkType.Image,
                    OwnershipStakes = new List<OwnershipStake>()
                    {
                        new()
                        {
                            Owner = "0x3aaf677ea4e72eebb92d2d5c3a92307ee789e24c",
                            Share = 100
                        }
                    },
                    WorkUri = "http://www.harrisonbarker.co.uk",
                    YearsExpire = 100
                });

                // await formsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(application.Id);
            }).Wait();
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
    }
}

public class DbPipelineBuilder
{
    public DbPipelineBuilder(IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContextPool<ContractContext>(builder =>
            builder.UseMySql(appSettings.ConnectionString, new MySqlServerVersion(new Version(8, 0, 23)),
                optionsBuilder => optionsBuilder.MigrationsAssembly("CRPL.Web"))
        );

        services.AddDbContextPool<ApplicationContext>(builder =>
            builder.UseMySql(appSettings.ConnectionString, new MySqlServerVersion(new Version(8, 0, 23)),
                optionsBuilder => optionsBuilder.MigrationsAssembly("CRPL.Web"))
        );

        services.AddSingleton<IContractRepository, ContractRepository>();
        services.AddSingleton<ICachedWorkRepository, CachedWorkRepository>();
        services.AddSingleton<IEventQueue, EventQueue>();
        services.AddSingleton<IVerificationQueue, VerificationQueue>();
        services.AddSingleton<IExpiryQueue, ExpiryQueue>();

        services.Configure<FormOptions>(o =>
        {
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
        });
    }
}