using MyLocations.Core.Shared;

namespace MyLocations.Core.Location
{
    public class AddLocationResult : Result
    {
        public ErrorType? Error { get; set; }

        public AddLocationResult(bool success) : base(success) { }

        public enum ErrorType
        {
            LocationAlreadyExists = 0, NoImagesDefined, ImagesAlreadyExists, CategoryNotFound
        }
    }

    public class SearchLocationResult : Result<Location>
    {
        public ErrorType? Error { get; set; }

        public SearchLocationResult(bool success, Location? value = null) : base(success, value) { }

        public enum ErrorType
        {
            LocationNotFound = 0, ImagesNotFound, CategoryNotFound
        }
    }
}
