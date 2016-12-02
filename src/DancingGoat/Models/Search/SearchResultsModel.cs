using PagedList;

namespace DancingGoat.Models.Search
{
    public class SearchResultsModel
    {
        public string Query { get; set; }

        public IPagedList<SearchResultItemModel> Items { get; set; }
    }
}