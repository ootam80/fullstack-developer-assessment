namespace MyLocations.Core.Location
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Location> Locations { get; set; }
    }
}
