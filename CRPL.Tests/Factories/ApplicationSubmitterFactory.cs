using System;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Web.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CRPL.Tests.Factories;

[TestFixture]
public class ApplicationSubmitterFactory
{
    public (ApplicationContext context, IServiceProvider Object, Mock<ICopyrightService> copyrightServiceMock, Mock<IAccountManagementService> accountManagementServiceMock, Mock<IRegistrationService> registrationServiceMock) Create(ApplicationContext context)
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ApplicationContext))).Returns(context);
        var copyrightServiceMock = new Mock<ICopyrightService>();
        serviceProvider.Setup(x => x.GetService(typeof(ICopyrightService))).Returns(copyrightServiceMock.Object);
        var accountManagementServiceMock = new Mock<IAccountManagementService>();
        serviceProvider.Setup(x => x.GetService(typeof(IAccountManagementService))).Returns(accountManagementServiceMock.Object);
        var registrationServiceMock = new Mock<IRegistrationService>();
        serviceProvider.Setup(x => x.GetService(typeof(IRegistrationService))).Returns(registrationServiceMock.Object);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        return (context, serviceProvider.Object, copyrightServiceMock, accountManagementServiceMock, registrationServiceMock);
    }
}