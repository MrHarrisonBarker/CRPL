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
    public (OwnershipSynchroniser, Mock<IBlockchainConnection> connectionMock, Mock<IContractRepository> contractRepoMock, Mock<IExpiryQueue> expiryQueueMock) Create(ApplicationContext context, Dictionary<string, object>? mappings)
    {
        var web3Mock = new MockWeb3(mappings);

        var connectionMock = new Mock<IBlockchainConnection>();
        connectionMock.Setup(x => x.Web3()).Returns(() => web3Mock.DummyWeb3);

        var contractRepoMock = new Mock<IContractRepository>();
        contractRepoMock.Setup(x => x.DeployedContract(CopyrightContract.Copyright)).Returns(new DeployedContract()
        {
            Address = "TEST CONTRACT"
        });

        var expiryQueueMock = new Mock<IExpiryQueue>();
        
        return (new OwnershipSynchroniser(
            new Logger<OwnershipSynchroniser>(new LoggerFactory()),
            context,
            connectionMock.Object,
            contractRepoMock.Object,
            expiryQueueMock.Object), connectionMock, contractRepoMock, expiryQueueMock);
    }
}