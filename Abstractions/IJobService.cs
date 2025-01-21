using JobScheduler.Models;

namespace ObserverScheduler.Abstractions
{
    public interface IJobService
    {
        Task InitiateWithTenant(string apiKey);
        Task<Guid> CreateJobAsync(JobModel model);
        Task UpdateJobAsync(JobModel model);
        Task KillJobAsync(Guid id);
        Task<JobModel?> GetJobInfoByIdAsync(Guid id);
        Task<JobModel?> GetJobInfoByNameAsync(string name);
        Task TriggerJobAsync(Guid id);
        Task TriggerJobByNameAsync(string name);
    }
}