using System;
using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CRPL.Tests.Factories;

public class FormsServiceFactory
{
    public (FormsService, Mock<IUserService> userServiceMock) Create(ApplicationContext context)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        var appSettings = Options.Create(new AppSettings()
        {
            EncryptionKey = "Bj3PtC818hVHkNH3nzI0HN8wJXY0oHdo"
        });

        var (copyrightService, connectionMock, contractRepoMock, expiryQueueMock) = new CopyrightServiceFactory().Create(context, null);
        var userServiceMock = new Mock<IUserService>();

        var serviceProvider = new Mock<IServiceProvider>();
        
        serviceProvider.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);
        serviceProvider.Setup(x => x.GetService(typeof(IUserService))).Returns(userServiceMock.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IBlockchainConnection))).Returns(connectionMock.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IContractRepository))).Returns(contractRepoMock.Object);
        serviceProvider.Setup(x => x.GetService(typeof(ICopyrightService))).Returns(copyrightService);
        serviceProvider.Setup(x => x.GetService(typeof(IAccountManagementService))).Returns(new Mock<IAccountManagementService>().Object);
        serviceProvider.Setup(x => x.GetService(typeof(IRegistrationService))).Returns(new Mock<IRegistrationService>().Object);

        userServiceMock.Setup(x => x.AreUsersReal(It.IsAny<List<string>>())).Returns(true);
        
        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        return (new FormsService(
            new Logger<FormsService>(new LoggerFactory()),
            context, mapper, appSettings,
            serviceProvider.Object), userServiceMock);
    }
}