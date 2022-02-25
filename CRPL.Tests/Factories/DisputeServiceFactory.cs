using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Tests.Mocks;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class DisputeServiceFactory
{
    public DisputeService Create(ApplicationContext context, Dictionary<string, object>? mappings)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);
        
        var web3Mock = new MockWeb3(mappings);

        var connectionMock = new Mock<IBlockchainConnection>();
        connectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);

        return new DisputeService(new Logger<DisputeService>(new LoggerFactory()), context, mapper, connectionMock.Object);
    }
}