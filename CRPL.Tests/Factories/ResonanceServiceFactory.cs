using System;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Core;
using CRPL.Web.Hubs;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class ResonanceServiceFactory
{
    public readonly ResonanceService ResonanceService;
    public readonly Mock<IServiceProvider> ServiceProviderMock = new();
    public readonly Mock<IHubContext<ResonanceHub, IResonanceHub>> HubContextMock = new();
    public readonly Mock<IQueryService> QueryServiceMock = new();

    public ResonanceServiceFactory(ApplicationContext applicationContext = null)
    {
        ServiceProviderMock.Setup(x => x.GetService(typeof(IHubContext<ResonanceHub, IResonanceHub>))).Returns(HubContextMock.Object);
        ServiceProviderMock.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(applicationContext);
        ServiceProviderMock.Setup(x => x.GetService(typeof(IQueryService))).Returns(QueryServiceMock.Object);

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping(new AppSettings())));
        var mapper = new Mapper(configuration);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(ServiceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        ServiceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

        ResonanceService = new ResonanceService(new Logger<ResonanceService>(new LoggerFactory()), mapper, ServiceProviderMock.Object);
    }
}