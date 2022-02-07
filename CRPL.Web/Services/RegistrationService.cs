using AutoMapper;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ILogger<UserService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;

    public RegistrationService(ILogger<UserService> logger, ApplicationContext context, IMapper mapper)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
    }

    public RegisteredWork StartRegistration(CopyrightRegistrationApplication application)
    {
        Logger.LogInformation("Started a copyright registration {Id}", application.Id);
        
        var registeredWork = new RegisteredWork()
        {
            AssociatedApplication = new List<Application>()
            {
                application
            },
            UserWorks = application.AssociatedUsers.Select(x => new UserWork()
            {
                UserAccount = x.UserAccount
            }).ToList(),
            Hash = application.WorkHash
        };

        Context.RegisteredWorks.Add(registeredWork);
        Context.SaveChanges();

        return registeredWork;
    }
}