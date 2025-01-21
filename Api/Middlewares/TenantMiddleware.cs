namespace ObserverScheduler.Api.Middlewares;

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/jobs"))
        {
            await _next(context);
            return;
        }

        if (context.Request.Headers.TryGetValue("api-key", out var apikey))
        {
            context.Items["apikey"] = apikey.ToString();
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("api-key header is required.");
            return;
        }

        await _next(context);
    }
}