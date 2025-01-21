using MongoDB.Driver;

namespace ObserverScheduler.Extensions;

public static class MongoClientExtension
{
    public static void AddMongoClientSingleton(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetValue<string>("MongoDbSettings:ConnectionString");
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            return new MongoClient(settings);
        });
    }
}
