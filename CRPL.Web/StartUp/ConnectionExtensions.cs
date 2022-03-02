using CRPL.Data.BlockchainUtils;

namespace CRPL.Web.StartUp;

public static class ConnectionExtensions
{
    public static IServiceCollection AddChainConnections(this IServiceCollection services)
    {
        services.AddScoped<IBlockchainConnection, BlockchainConnection>();
        return services.AddScoped<IIpfsConnection, IpfsConnection>();
    }
}