using MyLocations.Core.Location;
using MyLocations.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MyLocations.Infrastructure.Services.FourSquare
{
    /// <summary>
    /// Implement own Foursquare service as the Foursquare.Api nuget package does not expose the Places Api endpoints.
    /// As we are passing Api Keys, it's better to not call any third-party package implementation of Foursquare.
    /// </summary>
    public class FourSquareService : ISearchLocationService
    {
        private const string _apiBaseUrl = "https://api.foursquare.com/v3";

        private const string _apiKey = "FoursquareApiKey";

        private readonly IHttpClientFactory _factory;
        private readonly ISettingsRepository _settingsRepository;

        public FourSquareService(IHttpClientFactory factory, ISettingsRepository settingsRepository)
        {
            _factory = factory;
            _settingsRepository = settingsRepository;
        }

        public async Task<Core.Location.Location?> Search(string keyword, string region, Core.Location.Category category)
        {
            Core.Location.Location? location = null;

            PlacesApiResult? placesApiResult;

            using var client = _factory.CreateClient();

            var apiKey = (await _settingsRepository.GetSetting(_apiKey));

            if (apiKey is null) throw new Exception("Foursquare api key not defined.");

            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", apiKey.Value);

            // Take only the first location found for the given search criteria
            int searchLimit = 1;

            var query = new Dictionary<string, string>
            {
                { "query", keyword },
                { "near", region },
                { "categories", category.Id.ToString() },
                { "sort", "relevance" },
                { "limit", searchLimit.ToString() }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new UriBuilder($"{_apiBaseUrl}/places/search") { Query = string.Join("&", query.Select(q => $"{q.Key}={q.Value}")) }.Uri,
                Headers =
                {
                     { "Accept", "application/json" },
                }
            };

            using var response = client.Send(request);

            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound or System.Net.HttpStatusCode.BadRequest => null,
                    _ => throw new Exception($"An error occurred during the foursquare api request : {request}"),
                };
            }

            var stringContent = await response.Content.ReadAsStringAsync();

            placesApiResult = JsonConvert.DeserializeObject<PlacesApiResult>(stringContent);

            if (placesApiResult is null || (placesApiResult.Results?.FirstOrDefault() is var result && result is null)) return location;

            location = new Core.Location.Location
            {
                Category = category,
                Description = new Description
                {
                    Keyword = keyword,
                    Region = region,
                    Name = result.Name,
                    Address = result.Location.FormattedAddress
                },
                GeoCodes = new GeoCodes { Latitude = result.Geocodes.Main.Latitude, Longitude = result.Geocodes.Main.Longitude },
            };

            return location;
        }
    }
}
