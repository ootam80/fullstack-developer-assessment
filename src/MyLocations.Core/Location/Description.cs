namespace MyLocations.Core.Location
{
    public class Description
    {
        public string? Name { get; set; }

        public string? Region { get; set; }

        public string? Address { get; set; }

        /// <summary>
        /// We also store the keyword that was used to search for the location. 
        /// This can be helpful when filtering locations.
        /// </summary>
        public string Keyword { get; set; }

        public override string ToString()
        {
            string description;

            if (!string.IsNullOrWhiteSpace(Name))
                description = Name;
            else if (!string.IsNullOrWhiteSpace(Address))
                description = Address;
            else
                description = Keyword;

            return $"{description} - {Region}";
        }
    }
}
