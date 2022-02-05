using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class RegistrationServiceFactory
{
    public RegistrationService Create(ApplicationContext context)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        return new RegistrationService(new Mock<ILogger<UserService>>().Object, context, mapper);
    }
}