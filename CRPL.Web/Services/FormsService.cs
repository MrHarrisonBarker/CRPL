using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CRPL.Web.Services;

public class FormsService : IFormsService
{
    private readonly ILogger<FormsService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly AppSettings Options;

    public FormsService(ILogger<FormsService> logger, ApplicationContext context, IMapper mapper, IOptions<AppSettings> options)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        Options = options.Value;
    }

    public async Task<ApplicationViewModel> GetApplication(Guid id)
    {
        var application = await Context.Applications.FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        return application.Map(Mapper);
    }

    public Task DeleteApplication(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ApplicationViewModel>> GetMyApplications(Guid userId)
    {
        return await Context.Applications.Include(x => x.AssociatedUsers)
            .Where(x => x.AssociatedUsers.Any(u => u.UserId == userId))
            .Select(x => x.Map(Mapper)).ToListAsync();
    }

    public Task<ApplicationViewModel> Update(ApplicationInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}