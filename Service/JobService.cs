using JobScheduler.Models;
using MongoDB.Driver;
using ObserverScheduler.Abstractions;
using ObserverScheduler.Helper;
using ObserverScheduler.Models;

namespace ObserverScheduler.Service;

public class JobService : IJobService
{
    private readonly IMongoClient _mongoClient;
    private IMongoCollection<JobModel> _jobCollection;
    private IMongoCollection<JobLog> _logCollection;
    private readonly IUserService _userService;
    private UserViewModel _currentUser;
    private bool _isInitiated = false;

    public JobService(IMongoClient mongoClient, IUserService userService)
    {
        _mongoClient = mongoClient;
        _userService = userService;
    }

    public async Task InitiateWithTenant(string apiKey)
    {
        _currentUser = await _userService.GetUserByApiKey(apiKey);
        var database = _mongoClient.GetDatabase("ObserverScheduler");
        _jobCollection = database.GetCollection<JobModel>($"Jobs_{_currentUser.Id}");
        _logCollection = database.GetCollection<JobLog>($"JobLogs_{_currentUser.Id}");

        _isInitiated = true;
    }

    public async Task<Guid> CreateJobAsync(JobModel model)
    {
        if (await _jobCollection.Find(j => j.Name == model.Name).AnyAsync())
            throw new ArgumentException("Job name must be unique.");

        model.Id = Guid.NewGuid();

        model.Url = EncryptionHelper.Encrypt(model.Url, out string tag);
        model.UrlEncryptTag = tag;
        model.UserId = _currentUser.Id;

        await _jobCollection.InsertOneAsync(model);

        return model.Id;
    }

    public async Task UpdateJobAsync(JobModel model)
    {
        var result = await _jobCollection.ReplaceOneAsync(j => j.Id == model.Id, model);
        if (result.MatchedCount == 0)
            throw new KeyNotFoundException("Job not found.");
    }

    public async Task<JobModel?> GetJobInfoByIdAsync(Guid id)
    {
        var result = await _jobCollection.Find(j => j.Id == id).FirstOrDefaultAsync();
        if (result != null)
        {
            result.Url = EncryptionHelper.Decrypt(result.Url, result.UrlEncryptTag);
            result.Logs = await GetJobLogsAsync(id);
        }
        return result;
    }

    public async Task<JobModel?> GetJobInfoByNameAsync(string name)
    {
        var result = await _jobCollection.Find(j => j.Name == name).FirstOrDefaultAsync();
        if (result != null)
        {
            result.Url = EncryptionHelper.Decrypt(result.Url, result.UrlEncryptTag);
            result.Logs = await GetJobLogsAsync(result.Id);
        }
        return result;
    }

    public async Task<List<JobLog>> GetJobLogsAsync(Guid jobId)
    {
        return await _logCollection.Find(log => log.JobId == jobId)
            .SortByDescending(log => log.TriggeredAt)
            .Limit(10)
            .ToListAsync();
    }

    public async Task TriggerJobByNameAsync(string name)
    {
        var job = await GetJobInfoByNameAsync(name);
        if (job == null)
            throw new KeyNotFoundException("Job not found.");

        await ExecuteJobAsync(job);
    }

    public async Task KillJobAsync(Guid id)
    {
        ///TODO: We might consider to use soft delete instead of hard delete or may not delete logs.
        var result = await _jobCollection.DeleteOneAsync(j => j.Id == id);
        if (result.DeletedCount == 0)
            throw new KeyNotFoundException("Job not found.");

        await _logCollection.DeleteManyAsync(log => log.JobId == id);
    }

    public async Task TriggerJobAsync(Guid id)
    {
        var job = await GetJobInfoByIdAsync(id);
        if (job == null)
            throw new KeyNotFoundException("Job not found.");

        await ExecuteJobAsync(job);
    }

    private async Task ExecuteJobAsync(JobModel job)
    {
        var log = await JobTriggerHelper.TriggerJobAsync(job);

        await _logCollection.InsertOneAsync(log);
    }
}

public class JobTriggerHelper
{
    public static async Task<JobLog> TriggerJobAsync(JobModel job)
    {
        using var client = new HttpClient();
        var retries = 0;
        var success = false;
        var message = string.Empty;

        while (retries <= job.RetryCount && !success)
        {
            try
            {
                var response = await client.GetAsync(job.Url);
                response.EnsureSuccessStatusCode();
                success = true;
                message = "Success";
            }
            catch (Exception ex)
            {
                retries++;
                message = ex.Message;
            }
        }

        var log = new JobLog();
        log.TriggeredAt = DateTime.UtcNow;
        log.Status = success ? JobStatusConstant.Success : JobStatusConstant.Failed;
        log.Message = message;
        log.JobId = job.Id;

        return log;
    }
    
    
}

