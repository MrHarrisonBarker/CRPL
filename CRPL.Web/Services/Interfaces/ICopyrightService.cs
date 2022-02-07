using CRPL.Data.Account;

namespace CRPL.Web.Services.Interfaces;

public interface ICopyrightService
{
    public Task<List<RegisteredWorkViewModel>> GetUsersWorks(Guid id);
}