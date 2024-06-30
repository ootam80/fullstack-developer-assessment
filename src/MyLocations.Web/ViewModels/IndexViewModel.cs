using MyLocations.Core.Shared;

namespace MyLocations.Web.Models
{
    public class IndexViewModel
    {
        public PaginatedList<LocationImageViewModel> LocationImages { get; private set; }

        public string? Keyword { get; set; }

        public IEnumerable<PageButton> PagesButtons { get => _pageButtons.AsReadOnly(); }

        private readonly List<PageButton> _pageButtons = new();

        public IndexViewModel(PaginatedList<LocationImageViewModel> locationImages)
        {
            LocationImages = locationImages;

            BuildPageButtons(locationImages.CurrentPage, locationImages.TotalPages);
        }

        private void BuildPageButtons(int currentPage, int totalPages)
        {
            #region Pagination logic

            // If there are 5 pages, we display 5 buttons for the pages
            // If there are more than 5 pages, we display the first page btn, (current page - 1) btn, the current page btn, (the current page + 1) btn and the last page btn

            //   1 2 3 4 5
            //   
            //   1 2 .. 6         -> current page = 1
            //                    
            //   1 2 3 .. 6       -> current page = 2
            //                    
            //   1 2 3 4 .. 6     -> current page = 3
            //                    
            //   1 .. 3 4 5 6     -> current page = 4
            //                    
            //                    
            //   1 2 .. 40        -> current page = 1
            //                    
            //   1 2 3 .. 40      -> current page = 2
            //                    
            //   1 2 3 4 .. 40    -> current page = 3
            //   
            //   1 .. 3 4 5 .. 40 -> current page = 4

            #endregion

            if (totalPages > 5)
            {
                int firstPage = 1;
                int lastPage = totalPages;

                int previous = currentPage > firstPage ? currentPage - 1 : firstPage;
                int next = currentPage < lastPage ? currentPage + 1 : lastPage;

                if (currentPage > firstPage)
                {
                    _pageButtons.Add(new PageButton { Label = firstPage.ToString(), Page = firstPage });
                }

                if (previous > firstPage + 1)
                {
                    _pageButtons.Add(new PageButton { Label = ".." });
                }

                if (previous > firstPage)
                {
                    _pageButtons.Add(new PageButton { Label = previous.ToString(), Page = previous });
                }

                _pageButtons.Add(new PageButton { Label = currentPage.ToString(), Page = currentPage });

                if (next < lastPage)
                {
                    _pageButtons.Add(new PageButton { Label = next.ToString(), Page = next });
                }

                if (next + 1 < lastPage)
                {
                    _pageButtons.Add(new PageButton { Label = ".." });
                }

                if (currentPage < lastPage)
                {
                    _pageButtons.Add(new PageButton { Label = lastPage.ToString(), Page = lastPage });
                }
            }
            else
            {
                for (int i = 1; i <= totalPages; i++)
                    _pageButtons.Add(new PageButton { Label = i.ToString(), Page = i });
            }
        }
    }

    public class PageButton
    {
        public string Label { get; set; }
        public int? Page { get; set; }
    }
}
