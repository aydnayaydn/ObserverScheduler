namespace ObserverScheduler.Entities;

public class User
{
    public Guid Id { get; set; }

    public required string UserName { get; set; }

    public required string Role { get; set; } // admin, stackholder
    
    public string ApiKey { get; set; }

    public string ApiKeyTag { get; set; }

}