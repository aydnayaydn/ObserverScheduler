using ObserverScheduler.Models;

namespace ObserverScheduler.Abstractions;

public interface IUserService
{
    Task<UserViewModel> GetUserById(Guid userId);
    Task<UserViewModel> GetUserByApiKey(string apiKey);
    Task Create (UserCreateModel user);
    Task Delete (Guid userId);
}