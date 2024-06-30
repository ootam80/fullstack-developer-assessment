using MyLocations.Core.Location;
using MyLocations.Core.User;

namespace MyLocations.Core.Shared
{
    public interface IUnitOfWork
    {
        ILocationRepository LocationRepository { get; }

        IUserRepository UserRepository { get; }

        Task Commit();

        Task Rollback();
    }
}
