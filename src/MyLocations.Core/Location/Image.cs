namespace MyLocations.Core.Location
{
    public class Image
    {
        public string Id { get; set; }

        public string? Description { get; set; }

        public string Url { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}
