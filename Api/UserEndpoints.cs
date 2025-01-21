using ObserverScheduler.Abstractions;
using ObserverScheduler.Models;

namespace ObserverScheduler.Api;

public static class UserEndpoints
{
    public static void ConfigureUserEndpoints(this WebApplication app)
    {
        app.MapPost("/user", (UserCreateModel user, IUserService service)
            => service.Create(user))
        .WithName("CreateUser")
        .WithOpenApi();

        app.MapGet("/user/{id}", (Guid id, IUserService service)
            => service.GetUserById(id))
        .WithName("GetUserById")
        .WithOpenApi();

        app.MapDelete("/user/{id}", (Guid id, IUserService service)
            => service.Delete(id))
        .WithName("DeleteUser")
        .WithOpenApi();
    }
}