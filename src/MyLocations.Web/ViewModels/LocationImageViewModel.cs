using MyLocations.Core.Location;

namespace MyLocations.Web.Models
{
    /// <summary>
    /// This model is in fact representing a location image
    /// A location may have up to N images
    /// The N images of the location are flattened to a list of images
    /// Then the list of N images are displayed in the view
    /// This model represents an image of a location in the view
    /// </summary>
    public class LocationImageViewModel
    {
        /// <summary>
        /// The image Id
        /// </summary>
        public string? ImageId { get; set; }

        /// <summary>
        /// The location Id
        /// </summary>
        public int? LocationId { get; set; }

        /// <summary>
        /// The location description which is displayed as the image title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The image description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The image url.
        /// </summary>
        public string? ImageUrl { get; set; }

        public static LocationImageViewModel Create(Location location, Image image)
            => new ()
            {
                LocationId = location.Id,
                Title = location.Description.ToString(),
                Description = image.Description,
                ImageId = image.Id,
                ImageUrl = image.Url,
            };
    }
}
