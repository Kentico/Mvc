using PagedList;

using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultsModel
    {
        public string Query
        {
            get;
            set;
        }

        public IPagedList<SearchResultItem> Items
        {
            get;
            set;
        }
    }
}