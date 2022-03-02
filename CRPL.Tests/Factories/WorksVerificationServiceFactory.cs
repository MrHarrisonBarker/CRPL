using System.IO;
using CRPL.Data;
using CRPL.Web.Services;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.Works;
using Ipfs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CRPL.Tests.Factories;

public class WorksVerificationServiceFactory
{
    public (WorksVerificationService, Mock<IIpfsConnection> ipfsConnectionMock, CachedWorkRepository cachedWorkRepository) Create(ApplicationContext context)
    {
        var cachedWorkRepository = new CachedWorkRepository(new Logger<CachedWorkRepository>(new LoggerFactory()));
        
        cachedWorkRepository.Set(new byte[]{ 1, 1, 1, 1, 1 }, new byte[]{0}, "","");

        var appSettings = Options.Create(new AppSettings());

        var ipfsConnectionMock = new Mock<IIpfsConnection>();
        Cid id = new Cid()
        {
            Hash = "QmcLgUi4YEbzD7heFAWkKPZDphhEicXwm7ER82nahvXdqQ"
        };
        ipfsConnectionMock.Setup(x => x.AddFile(It.IsAny<MemoryStream>(), It.IsAny<string>())).ReturnsAsync(id);

        return (new WorksVerificationService(new Logger<WorksVerificationService>(new LoggerFactory()), context, appSettings, cachedWorkRepository, ipfsConnectionMock.Object), ipfsConnectionMock, cachedWorkRepository);
    }
}