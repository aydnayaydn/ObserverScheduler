using JobScheduler.Models;
using ObserverScheduler.Abstractions;

namespace ObserverScheduler.Api;

public static class JobEndpoints
{
    public static void ConfigureJobEndpoints(this WebApplication app)
    {
        // Endpointler
        app.MapPost("/jobs", async (JobModel model, IJobService jobService) =>
        {
            var jobId = await jobService.CreateJobAsync(model);
            return Results.Created($"/jobs/{jobId}", new { Id = jobId });
        }).WithName("CreateJob");

        app.MapPut("/jobs/{id}", async (string id, JobModel updatedModel, IJobService jobService) =>
        {
            try
            {
                await jobService.UpdateJobAsync(updatedModel);
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Error = ex.Message });
            }
        }).WithName("UpdateJob");

        app.MapDelete("/jobs/{name}", async (Guid id, IJobService jobService) =>
        {
            try
            {
                await jobService.KillJobAsync(id);
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Error = ex.Message });
            }
        }).WithName("DeleteJob");

        app.MapGet("/jobs/{id}", async (Guid id, IJobService jobService) =>
        {
            try
            {
                var jobInfo = await jobService.GetJobInfoByIdAsync(id);
                return Results.Ok(jobInfo);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Error = ex.Message });
            }
        }).WithName("GetJobInfo");

        app.MapPost("/jobs/{id}/trigger", async (Guid id, IJobService jobService) =>
        {
            try
            {
                await jobService.TriggerJobAsync(id);
                return Results.Ok(new { Message = $"Job '{id}' triggered successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { Error = ex.Message });
            }
        }).WithName("TriggerJobNow");
    }
}