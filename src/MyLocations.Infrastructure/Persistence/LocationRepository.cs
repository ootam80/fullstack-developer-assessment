using Microsoft.EntityFrameworkCore;
using MyLocations.Core.Location;
using MyLocations.Core.Shared;
using System.Linq.Expressions;

namespace MyLocations.Infrastructure.Persistence
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LocationRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task Add(Location location)
        {
            await _dbContext.Locations.AddAsync(location);
        }

        public async Task Delete(int id)
        {
            var location = await GetById(id);

            if (location is not null)
                _dbContext.Locations.Remove(location);
        }

        public async Task<Location?> GetById(int id)
        {
            return await _dbContext.Locations.Include(l => l.Images).SingleOrDefaultAsync(l => l.Id == id);
        }

        public async Task<ICollection<Location>> GetByImageDescription(string description)
        {
            return await _dbContext.Locations
                .Include(l => l.Images.Where(i => i.Description.Contains(description, StringComparison.OrdinalIgnoreCase)))
                .ToListAsync();
        }

        public async Task<ICollection<Image>> GetImagesByIds(IEnumerable<string> ids)
        {
            return await _dbContext.Images.Where(i => ids.Contains(i.Id)).ToListAsync();
        }

        public async Task<ICollection<Category>> GetCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _dbContext.Categories.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PaginatedList<Image>> GetImages(int? page = null, int? pageSize = null)
        {
            var query = (from location in _dbContext.Locations
                         from locationImage in location.Images
                         join image in _dbContext.Images on locationImage.Id equals image.Id
                         orderby location.CreatedAt descending
                         select new Image
                         {
                             Id = image.Id,
                             Description = image.Description,
                             Url = image.Url,
                             Locations = new List<Location> { location } // Retrieve first location for which the image was found
                         });

            int count = query.Count();

            int internalPage = 1, internalPageSize = count;

            bool mustApplyPagination = page.HasValue && pageSize.HasValue;

            if (mustApplyPagination)
            {
                internalPage = page!.Value;
                internalPageSize = pageSize!.Value;

                // TODO: Use keyset pagination for performance gain
                query = query.Skip((internalPage - 1) * internalPageSize).Take(internalPageSize);
            }

            var paginatedList = PaginatedList<Image>.Create(query.AsEnumerable().ToList(), count, internalPage, internalPageSize);

            return await Task.FromResult(paginatedList);
        }

        public async Task<PaginatedList<Image>> GetImagesByExpression(Expression<Func<Image, bool>> imageExpression, Expression<Func<Location, bool>> locationExpression, int? page = null, int? pageSize = null)
        {
            var filteredLocations = _dbContext.Locations.Where(locationExpression);
            var filteredImages = _dbContext.Images.Where(imageExpression);

            var queryLocations = (from location in filteredLocations
                                  from locationImage in location.Images
                                  join image in _dbContext.Images on locationImage.Id equals image.Id
                                  select new Image
                                  {
                                      Id = image.Id,
                                      Description = image.Description,
                                      Url = image.Url,
                                      Locations = new List<Location> { location } // Retrieve first location for which the image was found
                                  }).AsEnumerable();

            var queryImages = (from location in _dbContext.Locations
                               from locationImage in location.Images
                               join image in filteredImages on locationImage.Id equals image.Id
                               select new Image
                               {
                                   Id = image.Id,
                                   Description = image.Description,
                                   Url = image.Url,
                                   Locations = new List<Location> { location } // Retrieve first location for which the image was found
                               }).AsEnumerable();

            var mergedQuery = queryLocations.UnionBy(queryImages, q => q.Id);

            int count = mergedQuery.Count();

            int internalPage = 1, internalPageSize = count;

            bool mustApplyPagination = page.HasValue && pageSize.HasValue;

            if (mustApplyPagination)
            {
                internalPage = page!.Value;
                internalPageSize = pageSize!.Value;
                mergedQuery = mergedQuery.Skip((internalPage - 1) * internalPageSize).Take(internalPageSize);
            }

            var paginatedList = PaginatedList<Image>.Create(mergedQuery.ToList(), count, internalPage, internalPageSize);

            return await Task.FromResult(paginatedList);
        }

        public async Task<bool> Exists(Location location)
        {
            var locations = _dbContext.Locations.AsNoTracking().AsEnumerable();
            // A location is identified by the geo codes
            return await Task.FromResult(locations.SingleOrDefault(l => l.GeoCodes.Equals(location.GeoCodes)) is not null);
        }

        public async Task<Image?> GetImageById(string id)
        {
            return await _dbContext.Images.Include(i => i.Locations).SingleOrDefaultAsync(i => i.Id == id);
        }
    }
}
