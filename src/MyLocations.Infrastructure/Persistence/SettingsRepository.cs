
using Microsoft.EntityFrameworkCore;

namespace MyLocations.Infrastructure.Persistence
{
    internal class SettingsRepository : ISettingsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public SettingsRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;
        public async Task<Setting?> GetSetting(string key)
        {
            return await _dbContext.Settings.SingleOrDefaultAsync(s => s.Key == key);
        }

        public async Task<ICollection<Setting>> GetSettings(string[] keys)
        {
            return await Task.FromResult(_dbContext.Settings.Where(s => keys.Contains(s.Key)).ToList());
        }
    }
}
