using System.Collections.Generic;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Core.ChainSync.Synchronisers;
using CRPL.Web.Services.Background.SlientExpiry;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories.Synchronisers;

public class OwnershipSynchroniserFactory
{
    public readonly OwnershipSynchroniser OwnershipSynchroniser;
    public readonly Mock<IBlockchainConnection> BlockchainConnectionMock = new();
    public readonly Mock<IContractRepository> ContractRepositoryMock = new();
    public readonly Mock<IExpiryQueue> ExpiryQueueMock = new();

    public OwnershipSynchroniserFactory(ApplicationContext context, Dictionary<string, object>? mappings = null)
    {
        var web3Mock = new MockWeb3(mappings);
        
        BlockchainConnectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);
        
        ContractRepositoryMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        OwnershipSynchroniser = new OwnershipSynchroniser(new Logger<OwnershipSynchroniser>(new LoggerFactory()), context, BlockchainConnectionMock.Object, ContractRepositoryMock.Object, ExpiryQueueMock.Object);
    }
}