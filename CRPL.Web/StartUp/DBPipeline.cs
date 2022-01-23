using CRPL.Data.ContractDeployment;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.StartUp;

public static class DBExtensions
{
    public static DBPipeline AddDBPipeline(this IServiceCollection services, AppSettings settings) => new DBPipeline(services, settings);

    public class DBPipeline
    {
        public DBPipeline(IServiceCollection services, AppSettings appSettings)
        {
            services.AddDbContextPool<ContractContext>(builder => 
                builder.UseMySql(appSettings.ConnectionString, new MySqlServerVersion(new Version(8, 0, 23)), 
                    optionsBuilder => optionsBuilder.MigrationsAssembly("CRPL.Web"))
                );
        }
    }
}