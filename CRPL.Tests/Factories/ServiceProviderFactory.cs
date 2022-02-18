using System;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CRPL.Tests.Factories;

public class ServiceProviderWithContextFactory
{
    public async Task<(ApplicationContext, IServiceProvider)> Create(ApplicationContext context = null)
    {
        if (context == null)
        {
            context = new TestDbApplicationContextFactory().CreateContext();
        }
        
        var worksVerificationServiceMock = new Mock<IWorksVerificationService>();
        
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);
        serviceProvider.Setup(x => x.GetService(typeof(IWorksVerificationService))).Returns(worksVerificationServiceMock.Object);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

        return (context, serviceProvider.Object);
    }
}