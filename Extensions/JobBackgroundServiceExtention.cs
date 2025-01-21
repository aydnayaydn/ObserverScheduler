using MongoDB.Driver;
using ObserverScheduler.Service;

namespace ObserverScheduler.Extensions;
public static class JobBackgroundServiceExtention
{
    public static void AddJobBackgroundService(this IServiceCollection services)
    {
        services.AddHostedService(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var intervalInSeconds = configuration.GetValue<int>("CycleIntervalSeconds");
            return new JobBackgroundService(TimeSpan.FromSeconds(intervalInSeconds), mongoClient);
        });
    }
}
