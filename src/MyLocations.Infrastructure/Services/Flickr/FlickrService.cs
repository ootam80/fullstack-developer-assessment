using MyLocations.Core.Location;
using MyLocations.Infrastructure.Persistence;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MyLocations.Infrastructure.Services.Flickr
{
    /// <summary>
    /// As we are passing Api Keys, it's better to not call any third-party package implementation of Flickr.
    /// </summary>
    public class FlickrService : ISearchImageService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ISettingsRepository _settingsRepository;

        private const string _apiBaseUrl = "https://www.flickr.com/services/rest/";

        private const string _apiKeyIdentifier = "FlickApiKey";
        private const string _apiSecretIdentifier = "FlickApiSecret";

        private static Regex _jsonResultPattern = new(@"jsonFlickrApi\(({.*?})\)", RegexOptions.Compiled);

        public FlickrService(IHttpClientFactory factory, ISettingsRepository settingsRepository)
        {
            _factory = factory;
            _settingsRepository = settingsRepository;
        }

        public async Task<ICollection<Image>> Search(string keyword, string region, GeoCodes geoCodes, int numberOfImages)
        {
            FlickrApiResult? flickrApiResult;

            using var client = _factory.CreateClient();

            var settings = (await _settingsRepository.GetSettings([_apiKeyIdentifier, _apiSecretIdentifier]));

            var apiKey = settings.FirstOrDefault(s => s.Key == _apiKeyIdentifier);
            var apiSecret = settings.FirstOrDefault(s => s.Key == _apiSecretIdentifier);

            if (apiKey is null) throw new Exception("Flickr api key not defined.");

            if (apiSecret is null) throw new Exception("Flickr api secret not defined.");

            // We do not take image that are too old
            var minTakenDate = new DateTime(2017, 1, 1);

            var query = new Dictionary<string, string>
            {
                { "method", "flickr.photos.search"},
                { "api_key", apiKey.Value },
                { "api_secret", apiSecret.Value },
                { "lat", geoCodes.Latitude.ToString() },
                { "lon", geoCodes.Longitude.ToString() },
                { "format", "json"},
                { "text", $"{keyword} {region}" },
                { "sort", "relevance" },
                { "per_page", numberOfImages.ToString() },
                { "page", "1" },
                { "min_taken_date", minTakenDate.ToString("yyyy-MM-dd")}
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder($"{_apiBaseUrl}") { Query = string.Join("&", query.Select(q => $"{q.Key}={q.Value}")) }.Uri,
                Headers =
                {
                     { "Accept", "application/json" },
                }
            };

            using var response = client.Send(request);
            if (!response.IsSuccessStatusCode) throw new Exception($"An error occurred during the flickr api request : {request}");

            flickrApiResult = await GetJsonResult(response);

            if (flickrApiResult is null || (flickrApiResult.Photos?.Photo is var result && result is null || !result.Any())) return Enumerable.Empty<Image>().ToList();

            return result.Select(p => new Image
            {
                Id = $"{p.Server}-{p.Id}-{p.Secret}",
                Description = p.Title,
                Url = $"https://live.staticflickr.com/{p.Server}/{p.Id}_{p.Secret}.jpg"
            }).ToList();
        }

        private async Task<FlickrApiResult?> GetJsonResult(HttpResponseMessage flickrResponse)
        {
            // The json content from Flickr returned is as follows:
            //      jsonFlickrApi(
            //          {
            //              Your real json content
            //          }
            //      )

            var stringContent = await flickrResponse.Content.ReadAsStringAsync();
            var match = _jsonResultPattern.Match(stringContent);
            var jsonString = match.Success ? match.Groups[1].Value : "";

            if (string.IsNullOrWhiteSpace(jsonString))
                throw new Exception("The Flickr Api json result could not be extracted.");

            return JsonConvert.DeserializeObject<FlickrApiResult>(jsonString);
        }
    }
}
