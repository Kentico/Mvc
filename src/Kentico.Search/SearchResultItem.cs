using CMS.DataEngine;

namespace Kentico.Search
{
    /// <summary>
    /// Represents one search result.
    /// </summary>
    public sealed class SearchResultItem<TInfo> where TInfo : BaseInfo
    {
        /// <summary>
        /// Common search fields
        /// </summary>
        public SearchFields Fields
        {
            get;
            internal set;
        }


        /// <summary>
        /// Info object to get type specific data
        /// </summary>
        public TInfo Data
        {
            get;
            internal set;
        }
    }
}