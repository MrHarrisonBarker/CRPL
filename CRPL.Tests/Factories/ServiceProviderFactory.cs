using System;
using System.Threading.Tasks;
using CRPL.Data.Account;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CRPL.Tests.Factories;

public class ServiceProviderWithContextFactory
{
    public async Task<(ApplicationContext, IServiceProvider)> Create()
    {
        var context = new TestDbApplicationContextFactory().CreateContext();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

        return (context, serviceProvider.Object);
    }

    public async Task<(ApplicationContext, IServiceProvider)> Create(ApplicationContext context)
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

        return (context, serviceProvider.Object);
    }
}