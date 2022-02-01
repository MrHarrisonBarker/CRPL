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
        var cachedWorkRepository = new CachedWorkRepository(new Logger<CachedWorkRepository>(new LoggerFactory()));
        
        cachedWorkRepository.Set(new byte[]{0}, new byte[]{0}, "","");
        
        return new WorksVerificationService(new Logger<WorksVerificationService>(new LoggerFactory()), context, cachedWorkRepository);
    }
}