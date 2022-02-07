using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;

namespace CRPL.Web.Services.Interfaces;

public interface IRegistrationService
{
    public RegisteredWork StartRegistration(CopyrightRegistrationApplication application);
    public Task<RegisteredWork> CompleteRegistration(Guid applicationId);
}