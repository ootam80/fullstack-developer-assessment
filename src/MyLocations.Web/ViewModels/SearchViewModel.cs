namespace MyLocations.Web.Models
{
    public class SearchViewModel
    {
        public string? Keyword { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
