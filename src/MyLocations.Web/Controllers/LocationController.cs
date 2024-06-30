using Microsoft.AspNetCore.Mvc;
using MyLocations.Core.Location;
using MyLocations.Core.Shared;
using MyLocations.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MyLocations.Controllers
{
    public class LocationController : Controller
    {
        private readonly LocationService _locationService;
        private readonly ILogger<LocationController> _logger;

        private const int _pageSize = 10;

        public LocationController(LocationService locationService, ILogger<LocationController> logger)
        {
            _locationService = locationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ViewResult> Index([FromQuery] int page = 1, [FromQuery] string? keyword = null)
        {
            PaginatedList<Image> paginatedImages;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                paginatedImages = await _locationService.GetImages(page, _pageSize);
            }
            else
            {
                paginatedImages = await _locationService.GetImages(page, _pageSize, keyword);
            }

            var paginatedLocationImages = paginatedImages.ConvertToType(i => LocationImageViewModel.Create(i.Locations.First(), i));

            return View(new IndexViewModel(paginatedLocationImages) { Keyword = keyword });
        }

        [HttpGet]
        public async Task<IActionResult> ViewLocationImage([Required][FromQuery] string imageId, [Required][FromQuery] int locationId)
        {
            var image = await _locationService.GetImageById(imageId);

            var location = await _locationService.GetById(locationId);

            var vm = LocationImageViewModel.Create(location!, image!);

            return await Task.FromResult(PartialView("ViewImage", vm));
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([Required][FromBody] LocationViewModel locationVm)
        {
            var location = new Location
            {
                Category = new Category { Id = locationVm.Category.Id, Name = locationVm.Category.Name },
                CreatedAt = DateTime.UtcNow,
                Description = locationVm.Description,
                GeoCodes = locationVm.GeoCodes,
                Images = locationVm.LocationImages.Select(i => new Image { Id = i.ImageId!, Description = i.Description, Url = i.ImageUrl! }).ToList()
            };

            var result = await _locationService.Add(location);

            if (result.Success)
            {
                _logger.LogInformation($"The location {location.Description} was successfully added.");
                return Ok();
            }

            _logger.LogWarning($"Unsuccessful attempt to add location {location.Description}. Error code: {(int)result.Error!.Value} ");
            return BadRequest(result.Error!.Value);
        }

        [HttpGet]
        public async Task<ViewResult> Search(string? keyword)
        {
            var categories = await _locationService.GetCategories();

            return View("Search", new SearchViewModel { Keyword = keyword, Categories = categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }) });
        }

        [HttpGet]
        public async Task<IActionResult> SearchNewLocation([Required] string keyword, [Required] string region, [Required] int categoryId)
        {
            var result = await _locationService.Search(keyword, region, categoryId);

            var location = result.Value;

            if (result.Success) return Ok(new LocationViewModel
            {
                Description = location!.Description,
                Category = new CategoryViewModel { Id = location.Category.Id, Name = location.Category.Name },
                GeoCodes = location.GeoCodes,
                LocationImages = location.Images.Select(i => LocationImageViewModel.Create(location, i))
            });

            _logger.LogWarning($"Unsuccessful attempt to search location for the criteria: keyword={keyword} region={region} categoryId={categoryId}. Error code: {(int)result.Error!.Value} ");
            return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            });
        }
    }
}
