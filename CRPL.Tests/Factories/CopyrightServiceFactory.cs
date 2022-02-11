using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Data.Works;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class CopyrightServiceFactory
{
    public CopyrightService Create(ApplicationContext context)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);
        
        var cachedWorkRepository = new CachedWorkRepository(new Logger<CachedWorkRepository>(new LoggerFactory()));
        
        return new CopyrightService(new Logger<CopyrightService>(new LoggerFactory()), context, mapper, new Mock<IBlockchainConnection>().Object, new Mock<IContractRepository>().Object);
    }
}