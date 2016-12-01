using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;

namespace Kentico.Search
{
    /// <summary>
    /// Represents search result retrieved by the search service.
    /// </summary>
    public sealed class SearchResult
    {
        /// <summary>
        /// Collection of search results for the requested page number.
        /// </summary>
        public IEnumerable<SearchResultItem<BaseInfo>> Items
        {
            get;
            internal set;
        } = Enumerable.Empty<SearchResultItem<BaseInfo>>();


        /// <summary>
        /// Gets the total number of search results which can be retrieved from the search index (ignores paging).
        /// </summary>
        public int TotalNumberOfResults
        {
            get;
            internal set;
        }


    }
}
