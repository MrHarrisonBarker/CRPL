using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRPL.Tests.Factories;

public class UserServiceFactory
{
    public readonly UserService UserService;

    public UserServiceFactory(ApplicationContext context)
    {
        var appSettings = Options.Create(new AppSettings()
        {
            EncryptionKey = "Bj3PtC818hVHkNH3nzI0HN8wJXY0oHdo"
        });
        
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping(appSettings.Value)));
        var mapper = new Mapper(configuration);

        UserService = new UserService(new Logger<UserService>(new LoggerFactory()), context, mapper, appSettings);
    }
}