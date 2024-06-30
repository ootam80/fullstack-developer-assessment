using Newtonsoft.Json;

namespace MyLocations.Infrastructure.Services.FourSquare
{
    // These classes are obtained by taking a sample JSON result of PlacesApi and converting it into a C# class using an online JSON to C# class converter

    internal partial class PlacesApiResult
    {
        [JsonProperty("results")]
        internal Result[] Results { get; set; }

        [JsonProperty("context")]
        internal Context Context { get; set; }
    }

    internal class Context
    {
        [JsonProperty("geo_bounds")]
        internal GeoBounds GeoBounds { get; set; }
    }

    internal class GeoBounds
    {
        [JsonProperty("circle")]
        internal Circle Circle { get; set; }
    }

    internal class Circle
    {
        [JsonProperty("center")]
        internal Center Center { get; set; }

        [JsonProperty("radius")]
        internal long Radius { get; set; }
    }

    internal class Center
    {
        [JsonProperty("latitude")]
        internal double Latitude { get; set; }

        [JsonProperty("longitude")]
        internal double Longitude { get; set; }
    }

    internal class Result
    {
        [JsonProperty("fsq_id")]
        internal string FsqId { get; set; }

        [JsonProperty("categories")]
        internal Category[] Categories { get; set; }

        [JsonProperty("chains")]
        internal object[] Chains { get; set; }

        [JsonProperty("closed_bucket")]
        internal string ClosedBucket { get; set; }

        [JsonProperty("distance")]
        internal long Distance { get; set; }

        [JsonProperty("geocodes")]
        internal Geocodes Geocodes { get; set; }

        [JsonProperty("link")]
        internal string Link { get; set; }

        [JsonProperty("location")]
        internal Location Location { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("related_places")]
        internal RelatedPlaces RelatedPlaces { get; set; }

        [JsonProperty("timezone")]
        internal string Timezone { get; set; }
    }

    internal class BaseCategory
    {
        [JsonProperty("id")]
        internal long Id { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }
    }

    internal class Category : BaseCategory
    {
        [JsonProperty("short_name")]
        internal string ShortName { get; set; }

        [JsonProperty("plural_name")]
        internal string PluralName { get; set; }

        [JsonProperty("icon")]
        internal Icon Icon { get; set; }
    }

    internal class Icon
    {
        [JsonProperty("prefix")]
        internal Uri Prefix { get; set; }

        [JsonProperty("suffix")]
        internal string Suffix { get; set; }
    }

    internal class Geocodes
    {
        [JsonProperty("main")]
        internal Center Main { get; set; }
    }

    internal class Location
    {
        [JsonProperty("country")]
        internal string Country { get; set; }

        [JsonProperty("cross_street")]
        internal string CrossStreet { get; set; }

        [JsonProperty("formatted_address")]
        internal string FormattedAddress { get; set; }

        [JsonProperty("locality")]
        internal string Locality { get; set; }

        [JsonProperty("postcode")]
        internal string Postcode { get; set; }

        [JsonProperty("region")]
        internal string Region { get; set; }
    }

    internal class RelatedPlaces
    {
    }

    internal partial class PlacesApiResult
    {
        internal static PlacesApiResult FromJson(string json) => JsonConvert.DeserializeObject<PlacesApiResult>(json, Converter.Settings);
    }
}