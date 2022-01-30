using CRPL.Web.Services;
using CRPL.Data.Account;
using CRPL.Data.Works;
using Microsoft.Extensions.Logging;
using Moq;

namespace CRPL.Tests.Factories;

public class WorksVerificationServiceFactory
{
    public WorksVerificationService Create(ApplicationContext context)
    {
        var MockCachedWorksRepo = new Mock<ICachedWorkRepository>();
        return new WorksVerificationService(new Logger<WorksVerificationService>(new LoggerFactory()), context, MockCachedWorksRepo.Object);
    }
}