using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class CopyrightService : ICopyrightService
{
    private readonly ILogger<CopyrightService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;

    public CopyrightService(ILogger<CopyrightService> logger, ApplicationContext context, IMapper mapper)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
    }

    public async Task<List<RegisteredWorkWithAppsViewModel>> GetUsersWorks(Guid id)
    {
        Logger.LogInformation("Getting {Id}'s works", id);
        var works = await Context.RegisteredWorks
            .Include(x => x.AssociatedApplication)
            .Include(x => x.UserWorks)
            .Where(x => x.UserWorks.Any(u => u.UserId == id)).ToListAsync();
        
        return works.Select(x => Mapper.Map<RegisteredWorkWithAppsViewModel>(x)).ToList();
    }
}