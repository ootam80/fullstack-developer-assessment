namespace MyLocations.Core.Location
{
    public interface ISearchImageService
    {
        Task<ICollection<Image>> Search(string keyword, string region, GeoCodes geoCodes, int numberOfImages);
    }
}
