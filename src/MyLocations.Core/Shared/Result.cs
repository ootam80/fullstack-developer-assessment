namespace MyLocations.Core.Shared
{
    public class Result
    {
        public bool Success { get; private set; }

        public Result(bool success)
        {
            Success = success;
        }
    }

    public class Result<T> : Result where T : class
    {
        public Result(bool success, T? value = null) : base(success)
        {
            Value = value;
        }

        public T? Value { get; set; }
    }
}
