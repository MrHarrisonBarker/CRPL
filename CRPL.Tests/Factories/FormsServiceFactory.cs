using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services;
using CRPL.Web.Services.Interfaces;
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

        return new FormsService(
            new Logger<FormsService>(new LoggerFactory()), 
            context, mapper, appSettings, 
            new UserServiceFactory().Create(context),
            new Mock<IRegistrationService>().Object,
            new CopyrightServiceFactory().Create(context, null));
    }
}