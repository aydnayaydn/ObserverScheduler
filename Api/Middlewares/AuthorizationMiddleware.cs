using ObserverScheduler.Abstractions;
using ObserverScheduler.Common;

namespace ObserverScheduler.Api.Middlewares
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserService _userService;
        private readonly IJobService _jobService;

        public CustomAuthorizationMiddleware(RequestDelegate next, IUserService userService, IJobService jobService)
        {
            _next = next;
            _userService = userService;
            _jobService = jobService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/user") && !context.Request.Path.StartsWithSegments("/jobs"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("api-key", out var token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Authorization header is missing.");
                return;
            }
            
            var apiKey = context.Request.Headers["api-key"].ToString();

            var user = await _userService.GetUserByApiKey(apiKey);

            if (user == null)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid ApiKey.");
                return;
            }
            
            if (context.Request.Path.StartsWithSegments("/jobs"))
            {
                await _jobService.InitiateWithTenant(apiKey);
                await _next(context);
                return;
            }
            
            if (user.Role != RoleConstant.Admin)
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("You don't have permission to access this resource.");
                return;
            }
            else
            {
                await _next(context);
            }
        }
    }
}