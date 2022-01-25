using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.ContractDeployment;
using CRPL.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.StartUp;

public static class DbExtensions
{
    public static DbPipelineBuilder AddDbPipeline(this IServiceCollection services, AppSettings settings) => new DbPipelineBuilder(services, settings);
    public static UseSeedingBuilder UseSeeding(this WebApplication app) => new UseSeedingBuilder(app);
}

public class UseSeedingBuilder
{
    public UseSeedingBuilder(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        
        if(!context.UserAccounts.Any()) new UserAccountSeeder(context).Seed();
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
    }
}