using CRPL.Data;
using CRPL.Web.Services;
using CRPL.Web.Services.Background;
using CRPL.Web.Services.Interfaces;

namespace CRPL.Web.StartUp;

public static class ServiceExtensions
{
    public static ServicePipeline AddServicePipeline(this IServiceCollection services, AppSettings settings) => new ServicePipeline(services, settings);

    public class ServicePipeline
    {
        public ServicePipeline(IServiceCollection services, AppSettings appSettings)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorksVerificationService, WorksVerificationService>();
            services.AddScoped<IFormsService, FormsService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ICopyrightService, CopyrightService>();
            services.AddScoped<IQueryService, QueryService>();
            
            services.AddHostedService<BlockchainEventListener>();
            services.AddHostedService<EventProcessingService>();
        }
    }
}