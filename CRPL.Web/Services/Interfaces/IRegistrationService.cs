using CRPL.Data.Applications.ViewModels;

namespace CRPL.Web.Services.Interfaces;

public interface IRegistrationService
{
    public Task StartRegistration(CopyrightRegistrationApplication application);
}