using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class AccountManagementServiceFactory
{
    public readonly AccountManagementService AccountManagementService;
    public readonly Mock<IBlockchainConnection> BlockchainConnectionMock = new();
    public readonly Mock<IContractRepository> ContractRepositoryMock = new();
    public readonly Mock<IFormsService> FormsServiceMock = new();

    public AccountManagementServiceFactory(ApplicationContext context, Dictionary<string, object>? mappings = null)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping(new AppSettings())));
        var mapper = new Mapper(configuration);
        
        var web3Mock = new MockWeb3(mappings);

        BlockchainConnectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);

        ContractRepositoryMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        AccountManagementService = new AccountManagementService(new Logger<AccountManagementService>(new LoggerFactory()), context, mapper, BlockchainConnectionMock.Object,
            ContractRepositoryMock.Object, FormsServiceMock.Object);
    }
}