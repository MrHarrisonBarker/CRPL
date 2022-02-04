using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;

namespace CRPL.Web.Services.Interfaces;

public interface IFormsService
{
    public Task<ApplicationViewModel> GetApplication(Guid id);
    public Task DeleteApplication(Guid id);
    public Task<List<ApplicationViewModel>> GetMyApplications(Guid userId);
    public Task<ApplicationViewModel> Update(ApplicationInputModel inputModel);
}