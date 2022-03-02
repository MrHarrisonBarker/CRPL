using System.IO;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.Works;
using CRPL.Web.Services;
using Ipfs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CRPL.Tests.Factories;

public class WorksVerificationServiceFactory
{
    public readonly WorksVerificationService WorksVerificationService;
    public readonly Mock<IIpfsConnection> IpfsConnectionMock = new();
    public readonly Mock<ICachedWorkRepository> CachedWorkRepositoryMock = new();
    
    public WorksVerificationServiceFactory(ApplicationContext context)
    {
        var appSettings = Options.Create(new AppSettings());
        
        Cid id = new Cid()
        {
            Hash = "QmcLgUi4YEbzD7heFAWkKPZDphhEicXwm7ER82nahvXdqQ"
        };
        IpfsConnectionMock.Setup(x => x.AddFile(It.IsAny<MemoryStream>(), It.IsAny<string>())).ReturnsAsync(id);

        CachedWorkRepositoryMock.Setup(x => x.Get(It.IsAny<byte[]>())).Returns(new CachedWork()
        {
            Work = new byte[] { 0 },
            ContentType = "Image",
            FileName = "Filename"
        });
        
        WorksVerificationService = new WorksVerificationService(new Logger<WorksVerificationService>(new LoggerFactory()), context, appSettings, CachedWorkRepositoryMock.Object, IpfsConnectionMock.Object);
    }
}