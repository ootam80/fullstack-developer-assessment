namespace MyLocations.Core.Location
{
    public class Location
    {
        public int? Id { get; set; }

        public Description Description { get; set; }

        public Category Category { get; set; }

        public GeoCodes GeoCodes { get; set; }

        public ICollection<Image> Images { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
