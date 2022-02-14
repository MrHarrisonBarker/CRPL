using System;
using System.Collections.Generic;
using System.Net.Http;
using AutoMapper;
using Common.Logging;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Tests.Mocks;
using CRPL.Web.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nethereum.JsonRpc.Client;
using Nethereum.Model;
using Nethereum.Web3;

namespace CRPL.Tests.Factories;

public class QueryServiceFactory
{
    public QueryService Create(ApplicationContext context, Dictionary<string, object>? mappings)
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

        return new QueryService(
            new Logger<QueryService>(new LoggerFactory()),
            context,
            mapper,
            connectionMock.Object,
            contractRepoMock.Object);
    }
}