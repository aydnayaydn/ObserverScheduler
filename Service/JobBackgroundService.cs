using JobScheduler.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Quartz;

namespace ObserverScheduler.Service
{
    public class JobBackgroundService : BackgroundService
    {
        private readonly TimeSpan _interval;
        private readonly IMongoClient _mongoClient;

        public JobBackgroundService(TimeSpan interval, IMongoClient mongoClient)
        {
            _interval = interval;
            _mongoClient = mongoClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExecuteJobsAsync();
                }
                catch (Exception ex)
                {
                    // Log hataları (örn: Logger entegrasyonu ile)
                    Console.WriteLine($"Error in job execution: {ex.Message}");
                }

                // Belirtilen süre kadar bekler, bekleme sırasında token kontrolü yapılır
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task ExecuteJobsAsync()
        {
            var jobs = await GetJobsForCurrentUserAsync();
            foreach (var job in jobs)
            {
                // var currentDate = (await GetLastLogTriggeredAtAsync(job)).AddHours(job.UtcOffset);
                var currentDate = DateTime.UtcNow.AddHours(job.UtcOffset);
                var cronSchedule = new CronExpression(job.CronExpression);
                var nextOccurrence = cronSchedule.GetNextValidTimeAfter(currentDate.AddMinutes(-1));

                if (nextOccurrence.HasValue && nextOccurrence.Value <= currentDate)
                {
                    var log = await JobTriggerHelper.TriggerJobAsync(job);
                    await InsertJobLog(job, log);
                }
            }
        }

        private async Task InsertJobLog(JobModel job, JobLog log)
        {
            var database = _mongoClient.GetDatabase("ObserverScheduler");
            var collection = database.GetCollection<JobLog>($"JobLogs_{job.UserId}");
            await collection.InsertOneAsync(log);
        }

        private async Task<List<JobModel>> GetJobsForCurrentUserAsync()
        {
            var database = _mongoClient.GetDatabase("ObserverScheduler");
            var filter = new BsonDocument("name", new BsonDocument("$regex", $"^Jobs_"));
            var collections = await database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            var collectionNames = await collections.ToListAsync();

            var jobs = new List<JobModel>();
            var currentUtc = DateTime.UtcNow;

            foreach (var collectionName in collectionNames)
            {
                var collection = database.GetCollection<JobModel>(collectionName["name"].AsString);
                var filterBuilder = Builders<JobModel>.Filter;
                var collectionJobs = await collection.Find(job =>
                    job.ExpireDate > currentUtc)
                    .ToListAsync();

                jobs.AddRange(collectionJobs);
            }

            return jobs;
        }
    }
}