using System.Collections.ObjectModel;

namespace MyLocations.Core.Shared
{
    public class PaginatedList<T> : ReadOnlyCollection<T>
    {
        public bool HasNextPage { get => CurrentPage < TotalPages; }

        public bool HasPreviousPage { get => CurrentPage > 1; }

        public int TotalPages { get; private set; }

        public int CurrentPage { get; private set; }

        public static PaginatedList<T> Create(IList<T> list, int count, int currentPage, int pageSize)
        {
            var totalPages = (int)Math.Ceiling((double)count / pageSize);

            var paginatedList = new PaginatedList<T>(list) { TotalPages = totalPages, CurrentPage = currentPage };

            return paginatedList;
        }

        private PaginatedList(IList<T> list) : base(list) { }

        public PaginatedList<TOut> ConvertToType<TOut>(Func<T, TOut> converter) 
            => new(Items.Select(i => converter.Invoke(i)).ToList())
            {
                CurrentPage = CurrentPage,
                TotalPages = TotalPages
            };

    }
}
