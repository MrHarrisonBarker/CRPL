using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Data.Works;
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
        var cachedWorkRepository = new Mock<IContractRepository>().Object;

        return new RegistrationService(new Mock<ILogger<UserService>>().Object, context, mapper,new Mock<BlockchainConnection>().Object, cachedWorkRepository);
    }
}