using System;
using System.Collections.Generic;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Services.Background.Usage;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CRPL.Tests.Factories;

public class ServiceProviderWithContextFactory
{
    public readonly Mock<IServiceProvider> ServiceProviderMock;

    public readonly Mock<IUserService> UserServiceMock = new();
    public readonly Mock<ICopyrightService> CopyrightServiceMock = new();
    public readonly Mock<IWorksVerificationService> WorksVerificationServiceMock = new();
    public readonly Mock<IRegistrationService> RegistrationServiceMock = new();
    public readonly Mock<IQueryService> QueryServiceMock = new();
    public readonly Mock<IDisputeService> DisputeServiceMock = new();
    public readonly Mock<IAccountManagementService> AccountManagementServiceMock = new();
    public readonly Mock<IUsageQueue> UsageQueueMock = new();

    public readonly Mock<IBlockchainConnection> BlockchainConnectionMock = new();
    public readonly Mock<IContractRepository> ContractRepositoryMock = new();

    public ServiceProviderWithContextFactory(ApplicationContext context, Dictionary<string, object>? mappings = null)
    {
        ServiceProviderMock = new Mock<IServiceProvider>();
        
        ServiceProviderMock.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);
        
        ServiceProviderMock.Setup(x => x.GetService(typeof(IUserService))).Returns(UserServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(ICopyrightService))).Returns(CopyrightServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IWorksVerificationService))).Returns(WorksVerificationServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IRegistrationService))).Returns(RegistrationServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IQueryService))).Returns(QueryServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IDisputeService))).Returns(DisputeServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IAccountManagementService))).Returns(AccountManagementServiceMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IUsageQueue))).Returns(UsageQueueMock.Object);
        
        var web3Mock = new MockWeb3(mappings);
        
        BlockchainConnectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);
        
        ContractRepositoryMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        ServiceProviderMock.Setup(x => x.GetService(typeof(IBlockchainConnection))).Returns(BlockchainConnectionMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IContractRepository))).Returns(ContractRepositoryMock.Object);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(ServiceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        ServiceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
    }
}