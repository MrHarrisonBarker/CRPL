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

// A service for creating, updating and submitting applications
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

    // Get a single application
    public async Task<ApplicationViewModel> GetApplication(Guid id)
    {
        Logger.LogInformation("Getting application '{Id}'", id);
        var application = await Context.Applications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        
        // Map the application to its view model
        return application.Map(Mapper);
    }

    // Cancel an application
    public async Task CancelApplication(Guid id)
    {
        Logger.LogInformation("Canceling application '{Id}'", id);
        var application = await Context.Applications
            .Include(x => x.AssociatedWork)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (application == null) throw new ApplicationNotFoundException(id);
        
        // Remove from the database
        Context.Applications.Remove(application);
        await Context.SaveChangesAsync();
    }

    // Get all applications with a relationship to a specific user
    public async Task<List<ApplicationViewModel>> GetMyApplications(Guid userId)
    {
        Logger.LogInformation("Getting all application for {Id}", userId);
        return await Context.Applications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .Where(x => x.AssociatedUsers.Any(u => u.UserId == userId))
            .Select(x => x.Map(Mapper)).ToListAsync();
    }

    // Generic function for updating an application
    public async Task<T> Update<T>(ApplicationInputModel inputModel) where T : ApplicationViewModel
    {
        Logger.LogInformation("Updating application '{Id}'", inputModel.Id);

        var application = inputModel.Id == Guid.Empty ? null : await Context.Applications.FirstOrDefaultAsync(x => x.Id == inputModel.Id);

        // If the application doesn't exist create a new one
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

        // Update application
        application = await application.UpdateApplication(inputModel, ServiceProvider);
        application.Modified = DateTime.Now;

        await Context.SaveChangesAsync();

        // Push application updates to websocket
        await ResonanceService.PushApplicationUpdates(application);

        return (T)application.Map(Mapper);
    }

    // Generic function for submitting an application
    public async Task<O> Submit<T, O>(Guid id) where T : Application where O : ApplicationViewModel
    {
        Logger.LogInformation("Submitting {ApplicationType}", typeof(T).Name);
        var application = (await Context.Applications
            .Include(x => x.AssociatedWork)
            .Include(x => x.AssociatedUsers).ThenInclude(x => x.UserAccount)
            .FirstOrDefaultAsync(x => x.Id == id))!;

        if (application == null) throw new ApplicationNotFoundException(id);
        
        // Check application state
        if (application.Status == ApplicationStatus.Complete) throw new Exception("The application has already been complete!");
        if (application.Status == ApplicationStatus.Submitted) throw new Exception("The application has already been submitted!");

        // Submit
        var submittedApplication = (T)await application.SubmitApplication(ServiceProvider);
        submittedApplication.Modified = DateTime.Now;

        await Context.SaveChangesAsync();

        // Push application updates to websocket
        await ResonanceService.PushApplicationUpdates(submittedApplication);

        return (O)submittedApplication.Map(Mapper);
    }
}