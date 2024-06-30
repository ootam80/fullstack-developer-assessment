namespace MyLocations.Core.Location
{
    public interface ISearchLocationService
    {
        Task<Location?> Search(string keyword, string region, Category category);
    }
}
