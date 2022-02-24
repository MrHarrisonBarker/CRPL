using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;

namespace CRPL.Web.Services.Interfaces;

public interface IQueryService
{
    public Task<List<RegisteredWorkWithAppsViewModel>> GetUsersWorks(Guid id);
    public Task<RegisteredWorkWithAppsViewModel> GetWork(Guid id);
    public Task<List<RegisteredWorkViewModel>> GetAll(int from, int take = 100);
    public Task<List<RegisteredWorkViewModel>> Search(StructuredQuery query, int from, int take = 100);
    public Task<List<DisputeViewModelWithoutAssociated>> GetAllDisputes(int from, int take = 100);
    public Task<List<DisputeViewModel>> GetAllOwnersDisputes(Guid id);
}