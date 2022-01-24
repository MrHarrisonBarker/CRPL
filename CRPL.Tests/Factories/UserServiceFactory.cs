using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;

namespace CRPL.Tests.Factories;

public class UserServiceFactory
{
    public UserService Create(ApplicationContext context)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        return new UserService(new Logger<UserService>(new LoggerFactory()), context, mapper);
    }
}