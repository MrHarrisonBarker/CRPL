using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Core;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CRPL.Tests.Factories;

public class FormsServiceFactory
{
    public readonly FormsService FormsService;
    public readonly ServiceProviderWithContextFactory ServiceProviderWithContextFactory;
    public readonly Mock<IResonanceService> ResonanceServiceMock = new();

    public FormsServiceFactory(ApplicationContext context, Dictionary<string, object>? mappings = null)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping(new AppSettings())));
        var mapper = new Mapper(configuration);

        var appSettings = Options.Create(new AppSettings()
        {
            EncryptionKey = "Bj3PtC818hVHkNH3nzI0HN8wJXY0oHdo"
        });

        ServiceProviderWithContextFactory = new ServiceProviderWithContextFactory(context, mappings);

        FormsService = new FormsService(new Logger<FormsService>(new LoggerFactory()), context, mapper, appSettings, ServiceProviderWithContextFactory.ServiceProviderMock.Object,
            ResonanceServiceMock.Object);
    }
}