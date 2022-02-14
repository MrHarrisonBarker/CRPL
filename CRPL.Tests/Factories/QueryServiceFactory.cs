using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class QueryServiceFactory
{
    public QueryService Create(ApplicationContext context)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        return new QueryService(
            new Logger<QueryService>(new LoggerFactory()), 
            context, 
            mapper, 
            new Mock<IBlockchainConnection>().Object, 
            new Mock<IContractRepository>().Object);
    }
}