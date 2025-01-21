using ObserverScheduler.Abstractions;
using ObserverScheduler.Repositories;
using ObserverScheduler.Service;

namespace ObserverScheduler.Extensions;

public static class AppBusinessServicesExtension
{
    public static void AddAppBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJobService, JobService>();
    }
}