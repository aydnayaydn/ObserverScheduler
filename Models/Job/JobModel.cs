namespace JobScheduler.Models;

public class JobModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } 
    public string Description { get; set; }
    public string CronExpression { get; set; }
    public string Url { get; set; } // GET request yapılacak URL
    public DateTime ExpireDate { get; set; }
    public int UtcOffset { get; set; }
    public int RetryCount { get; set; }
    public int RetryDelay { get; set; } //milisecond
    public string UrlEncryptTag { get; set; }
    public Guid UserId { get; set; }
    public List<JobLog> Logs { get; set; } = new();
}

public class JobLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TriggeredAt { get; set; }
    public string Status { get; set; } 
    public string Message { get; set; } 
    public Guid JobId { get; set; }
}

public static class JobStatusConstant
{
    public const string Success = "Success";
    public const string Failed = "Failed";
}
