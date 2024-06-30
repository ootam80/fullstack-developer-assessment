using Microsoft.EntityFrameworkCore;
using MyLocations.Core.Location;
using MyLocations.Core.Shared;
using MyLocations.Core.User;

namespace MyLocations.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public ILocationRepository LocationRepository { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        // One improvement would be injecting a generic repository factory
        public UnitOfWork(ApplicationDbContext dbContext,
            ILocationRepository locationRepository,
            IUserRepository userRepository) 
        {
            _dbContext = dbContext;
            LocationRepository = locationRepository;
            UserRepository = userRepository;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task Rollback()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }

            await Task.FromResult(0);
        }
    }
}
