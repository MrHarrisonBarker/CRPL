using AutoMapper;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Core;
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
    private readonly IOptions<AppSettings> Options;
    private readonly IServiceProvider ServiceProvider;
    private readonly IResonanceService ResonanceService;

    public FormsService(
        ILogger<FormsService> logger,
        ApplicationContext context,
        IMapper mapper,
        IOptions<AppSettings> options,
        IServiceProvider serviceProvider,
        IResonanceService resonanceService)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        Options = options;
        ServiceProvider = serviceProvider;
        ResonanceService = resonanceService;
    }

    public async Task<ApplicationViewModel> GetApplication(Guid id)
    {
        Logger.LogInformation("Getting application '{Id}'", id);
        var application = await Context.Applications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        return application.Map(Mapper);
    }

    public async Task CancelApplication(Guid id)
    {
        Logger.LogInformation("Canceling application '{Id}'", id);
        var application = await Context.Applications
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        // if (application.AssociatedWork != null) Context.RegisteredWorks.Remove(application.AssociatedWork);
        Context.Applications.Remove(application);
        await Context.SaveChangesAsync();
    }

    public async Task<List<ApplicationViewModel>> GetMyApplications(Guid userId)
    {
        Logger.LogInformation("Getting all application for {Id}", userId);
        return await Context.Applications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
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
                case "DisputeViewModel":
                    application = new DisputeApplication { Id = new Guid() };
                    break;
                case "DeleteAccountViewModel":
                    application = new DeleteAccountApplication() { Id = new Guid() };
                    break;
            }

            if (application == null) throw new Exception("Could not determine the application type!");

            Context.Applications.Add(application);
        }
        else Context.Applications.Update(application);

        application = await application.UpdateApplication(inputModel, ServiceProvider);
        application.Modified = DateTime.Now;

        await Context.SaveChangesAsync();

        await ResonanceService.PushApplicationUpdates(application);

        return (T)application.Map(Mapper);
    }

    public async Task<O> Submit<T, O>(Guid id) where T : Application where O : ApplicationViewModel
    {
        Logger.LogInformation("Submitting {ApplicationType}", typeof(T).Name);
        var application = (await Context.Applications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id))!;

        if (application == null) throw new ApplicationNotFoundException(id);

        if (application.Status == ApplicationStatus.Complete) throw new Exception("The application has already been complete!");
        if (application.Status == ApplicationStatus.Submitted) throw new Exception("The application has already been submitted!");

        var submittedApplication = (T)await application.SubmitApplication(ServiceProvider);
        submittedApplication.Modified = DateTime.Now;

        await Context.SaveChangesAsync();

        await ResonanceService.PushApplicationUpdates(submittedApplication);

        return (O)submittedApplication.Map(Mapper);
    }
}