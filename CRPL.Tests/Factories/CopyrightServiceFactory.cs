using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Services;
using CRPL.Web.Services.Background;
using CRPL.Web.Services.Background.SlientExpiry;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class CopyrightServiceFactory
{
    public CopyrightService Create(ApplicationContext context, Dictionary<string, object>? mappings)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        var web3Mock = new MockWeb3(mappings);

        var connectionMock = new Mock<IBlockchainConnection>();
        connectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);

        var contractRepoMock = new Mock<IContractRepository>();
        contractRepoMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        var expiryQueueMock = new Mock<IExpiryQueue>();
        
        return new CopyrightService(
            new Logger<CopyrightService>(new LoggerFactory()),
            context,
            mapper,
            connectionMock.Object,
            contractRepoMock.Object,
            expiryQueueMock.Object);
    }
}