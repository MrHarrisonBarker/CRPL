using System.Collections.Generic;
using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Web.Services;
using Microsoft.Extensions.Logging;

namespace CRPL.Tests.Factories;

public class DisputeServiceFactory
{
    public DisputeService Create(ApplicationContext context, Dictionary<string, object>? mappings)
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapping()));
        var mapper = new Mapper(configuration);

        return new DisputeService(new Logger<DisputeService>(new LoggerFactory()), context, mapper);
    }
}