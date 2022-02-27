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
    public (AccountManagementService, Mock<IFormsService> formsServiceMock) Create(ApplicationContext context, Dictionary<string, object>? mappings)
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

        var formsServiceMock = new Mock<IFormsService>();
        
        return (new AccountManagementService(
            new Logger<AccountManagementService>(new LoggerFactory()),
            context,
            mapper,
            connectionMock.Object,
            contractRepoMock.Object,
            formsServiceMock.Object), formsServiceMock);
    }
}