using MyLocations.Core.Shared;
using System.Linq.Expressions;

namespace MyLocations.Core.Location
{
    public class LocationService
    {
        private readonly ISearchLocationService _searchLocationService;
        private readonly ISearchImageService _searchImageService;
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(ISearchLocationService searchLocationService,
            ISearchImageService searchPhotoService,
            IUnitOfWork unitOfWork)
        {
            _searchLocationService = searchLocationService;
            _searchImageService = searchPhotoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<AddLocationResult> Add(Location location)
        {
            if (await _unitOfWork.LocationRepository.Exists(location))
            {
                return new AddLocationResult(false) { Error = AddLocationResult.ErrorType.LocationAlreadyExists };
            }

            if (location.Images.Count == 0)
            {
                return new AddLocationResult(false) { Error = AddLocationResult.ErrorType.NoImagesDefined };
            }

            var imageIds = location.Images.Select(i => i.Id);

            // There could be other locations with the same images but with slight differences in location description
            var existingImageIds = (await _unitOfWork.LocationRepository.GetImagesByIds(imageIds)).Select(i => i.Id);

            // We still add the new images and prevent addition of duplicate location images
            location.Images = location.Images.ExceptBy(existingImageIds, img => img.Id).ToList();

            if (location.Images.Count == 0)
            {
                return new AddLocationResult(false) { Error = AddLocationResult.ErrorType.ImagesAlreadyExists };
            }

            var category = await _unitOfWork.LocationRepository.GetCategoryById(location.Category.Id);

            if (category is null)
            {
                return new AddLocationResult(false) { Error = AddLocationResult.ErrorType.CategoryNotFound };
            }

            location.Category = category;

            await _unitOfWork.LocationRepository.Add(location);

            await _unitOfWork.Commit();

            return new AddLocationResult(true);
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.LocationRepository.Delete(id);
            await _unitOfWork.Commit();
        }

        public async Task<PaginatedList<Image>> GetImages(int page, int pageSize)
        {
            return await _unitOfWork.LocationRepository.GetImages(page, pageSize);
        }

        public async Task<PaginatedList<Image>> GetImages(int page, int pageSize, string keyword)
        {
            // Check for keyword in location's description and images' description

            Expression<Func<Location, bool>> locationPredicate = l => l.Description.Name.Contains(keyword)
                || l.Description.Address.Contains(keyword)
                || l.Description.Region.Contains(keyword)
                || l.Description.Keyword.Contains(keyword);

            Expression<Func<Image, bool>> imagePredicate = i => i.Description.Contains(keyword);

            var locationsByExpression = await _unitOfWork.LocationRepository.GetImagesByExpression(imagePredicate, locationPredicate, page, pageSize);

            return locationsByExpression;
        }

        public async Task<Image?> GetImageById(string imageId)
        {
            return await _unitOfWork.LocationRepository.GetImageById(imageId);
        }

        public async Task<Location?> GetById(int id) => await _unitOfWork.LocationRepository.GetById(id);

        public async Task<IEnumerable<Category>> GetCategories() => await _unitOfWork.LocationRepository.GetCategories();

        public async Task<SearchLocationResult> Search(string keyword, string region, int categoryId, int numberOfImages = 4)
        {
            var category = await _unitOfWork.LocationRepository.GetCategoryById(categoryId);

            if (category is null) { return new SearchLocationResult(false) { Error = SearchLocationResult.ErrorType.CategoryNotFound }; }

            Location? location = await _searchLocationService.Search(keyword, region, category);

            if (location is null) { return new SearchLocationResult(false) { Error = SearchLocationResult.ErrorType.LocationNotFound }; }

            var images = await _searchImageService.Search(keyword, region, location.GeoCodes, numberOfImages);

            if (images.Count == 0) { return new SearchLocationResult(false) { Error = SearchLocationResult.ErrorType.ImagesNotFound }; }

            location.Images = images;

            return new SearchLocationResult(true, location);
        }
    }
}
