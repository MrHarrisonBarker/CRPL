using System;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CRPL.Tests.Factories;

public class FormsServiceFactory
{
    public FormsService Create(ApplicationContext context)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        var appSettings = Options.Create(new AppSettings()
        {
            EncryptionKey = "Bj3PtC818hVHkNH3nzI0HN8wJXY0oHdo"
        });

        var copyrightService = new CopyrightServiceFactory().Create(context, null);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);
        serviceProvider.Setup(x => x.GetService(typeof(ICopyrightService))).Returns(copyrightService.Item1);
        serviceProvider.Setup(x => x.GetService(typeof(IAccountManagementService))).Returns(new Mock<IAccountManagementService>().Object);
        serviceProvider.Setup(x => x.GetService(typeof(IRegistrationService))).Returns(new Mock<IRegistrationService>().Object);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        return new FormsService(
            new Logger<FormsService>(new LoggerFactory()),
            context, mapper, appSettings,
            serviceProvider.Object);
    }
}