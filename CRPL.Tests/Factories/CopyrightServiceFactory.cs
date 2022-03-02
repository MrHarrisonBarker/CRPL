using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Services;
using CRPL.Web.Services.Background.SlientExpiry;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class CopyrightServiceFactory
{
    public readonly CopyrightService CopyrightService;
    public readonly Mock<IBlockchainConnection> BlockchainConnectionMock = new();
    public readonly Mock<IContractRepository> ContractRepositoryMock = new();
    public readonly Mock<IExpiryQueue> ExpiryQueueMock = new();

    public CopyrightServiceFactory(ApplicationContext context, Dictionary<string, object>? mappings = null)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        var web3Mock = new MockWeb3(mappings);

        BlockchainConnectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);

        ContractRepositoryMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        CopyrightService = new CopyrightService(new Logger<CopyrightService>(new LoggerFactory()), context, mapper, BlockchainConnectionMock.Object, ContractRepositoryMock.Object,
            ExpiryQueueMock.Object);
    }
}