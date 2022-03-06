using System;
using AutoMapper;
using CRPL.Data;
using CRPL.Web.Core;
using CRPL.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class ResonanceServiceFactory
{
    public readonly ResonanceService ResonanceService;
    public readonly Mock<IServiceProvider> ServiceProviderMock = new();
    public readonly Mock<IHubContext<ApplicationsHub, IApplicationsHub>> HubContextMock = new();

    public ResonanceServiceFactory()
    {
        ServiceProviderMock.Setup(x => x.GetService(typeof(IHubContext<ApplicationsHub, IApplicationsHub>))).Returns(HubContextMock.Object);

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(ServiceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        ServiceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

        ResonanceService = new ResonanceService(new Logger<ResonanceService>(new LoggerFactory()), mapper, ServiceProviderMock.Object);
    }
}