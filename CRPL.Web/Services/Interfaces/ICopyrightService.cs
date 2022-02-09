using CRPL.Data.Account;
using CRPL.Data.Applications;

namespace CRPL.Web.Services.Interfaces;

public interface ICopyrightService
{
    public Task<List<RegisteredWorkWithAppsViewModel>> GetUsersWorks(Guid id);
    public Task<RegisteredWork> GetWork(Guid id);
    public Task AttachWorkToApplicationAndCheckValid(Guid id, Application applicationId);
    public Task<OwnershipRestructureApplication> ProposeRestructure(OwnershipRestructureApplication application);
}