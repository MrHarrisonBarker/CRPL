using CRPL.Data.Account;
using CRPL.Web.Services;

namespace CRPL.Tests.Factories;

public class RegistrationServiceFactory
{
    public RegistrationService Create(ApplicationContext context)
    {
        return new RegistrationService();
    }
}