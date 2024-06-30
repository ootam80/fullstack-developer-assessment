using MyLocations.Core.Location;

namespace MyLocations.Web.Models
{
    public class LocationViewModel
    {
        public Description Description { get; set; }

        public CategoryViewModel Category { get; set; }

        public GeoCodes GeoCodes { get; set; }

        public IEnumerable<LocationImageViewModel> LocationImages { get; set; }
    }
}
