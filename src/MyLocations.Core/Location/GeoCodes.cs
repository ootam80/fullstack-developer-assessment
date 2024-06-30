namespace MyLocations.Core.Location
{
    public class GeoCodes : IEquatable<GeoCodes>
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool Equals(GeoCodes? geoCode)
        {
            if (geoCode is null) return false;

            return this.Latitude == geoCode.Latitude && this.Longitude == geoCode.Longitude;
        }
    }
}
