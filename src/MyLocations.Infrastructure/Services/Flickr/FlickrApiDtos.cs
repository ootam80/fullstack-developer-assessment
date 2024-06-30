using Newtonsoft.Json;

namespace MyLocations.Infrastructure.Services.Flickr
{
    // These classes are obtained by taking a sample JSON result of Flickr Api and converting it into a C# class using an online JSON to C# class converter

    internal partial class FlickrApiResult
    {
        [JsonProperty("photos")]
        internal Photos Photos { get; set; }

        [JsonProperty("stat")]
        internal string Stat { get; set; }
    }

    internal class Photos
    {
        [JsonProperty("page")]
        internal long Page { get; set; }

        [JsonProperty("pages")]
        internal long Pages { get; set; }

        [JsonProperty("perpage")]
        internal long Perpage { get; set; }

        [JsonProperty("total")]
        internal long Total { get; set; }

        [JsonProperty("photo")]
        internal Photo[] Photo { get; set; }
    }

    internal class Photo
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("owner")]
        internal string Owner { get; set; }

        [JsonProperty("secret")]
        internal string Secret { get; set; }

        [JsonProperty("server")]
        [JsonConverter(typeof(ParseStringConverter))]
        internal long Server { get; set; }

        [JsonProperty("farm")]
        internal long Farm { get; set; }

        [JsonProperty("title")]
        internal string Title { get; set; }

        [JsonProperty("isinternal")]
        internal long Isinternal { get; set; }

        [JsonProperty("isfriend")]
        internal long Isfriend { get; set; }

        [JsonProperty("isfamily")]
        internal long Isfamily { get; set; }
    }

    internal partial class FlickrApiResult
    {
        internal static FlickrApiResult FromJson(string json) => JsonConvert.DeserializeObject<FlickrApiResult>(json, Converter.Settings);
    }
}
