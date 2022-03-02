using Ipfs;
using Ipfs.CoreApi;
using Ipfs.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRPL.Data.BlockchainUtils;

public interface IIpfsConnection
{
    public Task<Cid> AddFile(MemoryStream data, string fileName);
}

public class IpfsConnection : IIpfsConnection
{
    private readonly ILogger<IpfsConnection> Logger;
    private readonly AppSettings AppSettings;
    private readonly IpfsClient Client;

    public IpfsConnection(ILogger<IpfsConnection> logger, IOptions<AppSettings> appSettings)
    {
        Logger = logger;
        AppSettings = appSettings.Value;

        Client = new IpfsClient(AppSettings.IpfsHost);
    }

    public async Task<Cid> AddFile(MemoryStream data, string fileName)
    {
        Logger.LogInformation("Adding {Name} to ipfs", fileName);

        return (await Client.FileSystem.AddAsync(data, fileName, new AddFileOptions
        {
            Pin = true,
        })).Id;
    }
}