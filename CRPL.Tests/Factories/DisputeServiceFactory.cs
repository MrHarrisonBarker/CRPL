using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Core;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class DisputeServiceFactory
{
    public readonly DisputeService DisputeService;
    public readonly Mock<IFormsService> FormsServiceMock = new();
    public readonly Mock<IBlockchainConnection> BlockchainConnectionMock = new();
    public readonly Mock<IContractRepository> ContractRepositoryMock = new();
    public readonly Mock<IResonanceService> ResonanceServiceMock = new();

    public DisputeServiceFactory(ApplicationContext context, Dictionary<string, object>? mappings = null)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        var web3Mock = new MockWeb3(mappings);

        BlockchainConnectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);

        ContractRepositoryMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        DisputeService = new DisputeService(new Logger<DisputeService>(new LoggerFactory()), context, mapper, BlockchainConnectionMock.Object, ContractRepositoryMock.Object, FormsServiceMock.Object, ResonanceServiceMock.Object);
    }
}