using System.Collections.Generic;

using Kentico.Search;
using DancingGoat.Models.Pager;

namespace DancingGoat.Models.Search
{
    public class SearchResults : IPagedDataSource
    {
        #region "Properties"

        public string Query
        {
            get;
            set;
        }

        public IEnumerable<SearchResultItem> Items
        {
            get;
            set;
        }

        #endregion


        #region "IPagedDataSource Implementation"

        public int PageIndex
        {
            get;
            set;
        }


        public int PageSize
        {
            get;
            set;
        }


        public int TotalItemCount
        {
            get;
            set;
        }

        #endregion
    }
}