namespace ObserverScheduler.Api;

public static class Endpoints
{
    public static void ConfigureEndpoints(this WebApplication app)
    {
        app.Map("/", () =>
        {
            return "v.1.0.0";
        });

        app.MapGet("/connection", () =>
        {
            return "ONLINE";
        })
        .WithName("connection")
        .WithOpenApi();
    }
}