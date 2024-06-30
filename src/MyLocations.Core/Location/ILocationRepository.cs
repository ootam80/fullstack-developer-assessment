using MyLocations.Core.Shared;
using System.Linq.Expressions;

namespace MyLocations.Core.Location
{
    public interface ILocationRepository
    {
        Task<PaginatedList<Image>> GetImages(int? page = null, int? pageSize = null);

        Task<PaginatedList<Image>> GetImagesByExpression(Expression<Func<Image, bool>> imageExpression, Expression<Func<Location, bool>> locationExpression, int? page = null, int? pageSize = null);

        Task<ICollection<Location>> GetByImageDescription(string description);

        Task<Location?> GetById(int id);

        Task<bool> Exists(Location location);

        Task<ICollection<Category>> GetCategories();

        Task<Category?> GetCategoryById(int id);

        Task Add(Location location);

        Task Delete(int id);

        Task<ICollection<Image>> GetImagesByIds(IEnumerable<string> ids);

        Task<Image?> GetImageById(string id);
    }
}
