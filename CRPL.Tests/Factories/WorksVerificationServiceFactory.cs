using CRPL.Web.Services;
using CRPL.Data.Account;
using Microsoft.Extensions.Logging;

namespace CRPL.Tests.Factories;

public class WorksVerificationServiceFactory
{
    public WorksVerificationService Create(ApplicationContext context)
    {
        return new WorksVerificationService(new Logger<WorksVerificationService>(new LoggerFactory()), context);
    }
}