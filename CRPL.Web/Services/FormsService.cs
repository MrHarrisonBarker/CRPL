using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications;
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
    private readonly IUserService UserService;
    private readonly IRegistrationService RegistrationService;
    private readonly AppSettings Options;

    public FormsService(
        ILogger<FormsService> logger,
        ApplicationContext context,
        IMapper mapper,
        IOptions<AppSettings> options,
        IUserService userService,
        IRegistrationService registrationService)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        UserService = userService;
        RegistrationService = registrationService;
        Options = options.Value;
    }

    public async Task<ApplicationViewModel> GetApplication(Guid id)
    {
        Logger.LogInformation("Getting application '{Id}'", id);
        var application = await Context.Applications.Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount).FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        return application.Map(Mapper);
    }

    public async Task CancelApplication(Guid id)
    {
        Logger.LogInformation("Canceling application '{Id}'", id);
        var application = await Context.Applications.FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        Context.Applications.Remove(application);
        await Context.SaveChangesAsync();
    }

    public async Task<List<ApplicationViewModel>> GetMyApplications(Guid userId)
    {
        Logger.LogInformation("Getting all application for {Id}", userId);
        return await Context.Applications.Include(x => x.AssociatedUsers)
            .Where(x => x.AssociatedUsers.Any(u => u.UserId == userId))
            .Select(x => x.Map(Mapper)).ToListAsync();
    }

    public async Task<T> Update<T>(ApplicationInputModel inputModel) where T : ApplicationViewModel
    {
        Logger.LogInformation("Updating application '{Id}'", inputModel.Id);

        var application = inputModel.Id == Guid.Empty ? null : await Context.Applications.FirstOrDefaultAsync(x => x.Id == inputModel.Id);

        if (application == null)
        {
            Logger.LogInformation("Didn't find application so making a new one");

            switch (typeof(T).Name)
            {
                case "CopyrightRegistrationViewModel":
                    application = new CopyrightRegistrationApplication { Id = new Guid() };
                    break;
                case "OwnershipRestructureViewModel":
                    application = new OwnershipRestructureApplication { Id = new Guid() };
                    break;
            }

            if (application == null) throw new Exception("Could not determine the application type!");
        }

        Context.Applications.Update(application);
        application.Update(inputModel, Mapper, UserService);
        application.Modified = DateTime.Now;

        await Context.SaveChangesAsync();

        return (T)application.Map(Mapper);
    }

    public async Task<O> Submit<T, O>(Guid id) where T : Application where O : ApplicationViewModel
    {
        var application = (await Context.Applications
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id))!;

        if (application == null) throw new ApplicationNotFoundException(id);

        var submittedApplication = (T)application.Submit(RegistrationService);
        submittedApplication.Modified = DateTime.Now;

        await Context.SaveChangesAsync();

        return (O)submittedApplication.Map(Mapper);
    }
}