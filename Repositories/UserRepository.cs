using Microsoft.EntityFrameworkCore;
using ObserverScheduler.Abstractions;
using ObserverScheduler.Common;
using ObserverScheduler.Data;
using ObserverScheduler.Entities;
using ObserverScheduler.Helper;

namespace ObserverScheduler.Repositories
{
    //no need a generic repository
    public class UserRepository : IUserRepository
    {
        private readonly MainContext _dbContext;

        public UserRepository(MainContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _dbContext.Set<User>().FindAsync(id) ?? throw new Exception("User not found");
            return user;
        } 
        

        public async Task<User> GetUserByApiKey(string apiKey)
        {
            string tag;
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.ApiKey == EncryptionHelper.Encrypt(apiKey, out tag));
            return user;
        }

        public async Task Create(User user)
        {
            user.ApiKey = EncryptionHelper.Encrypt(ApiKeyGenerator.GenerateAPIKey(), out string tag);
            user.ApiKeyTag = tag;
            await _dbContext.Set<User>().AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var user = await _dbContext.Set<User>().FindAsync(id);
            if (user != null)
            {
                _dbContext.Set<User>().Remove(user);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}