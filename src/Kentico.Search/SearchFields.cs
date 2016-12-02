using System;

namespace Kentico.Search
{
    /// <summary>
    /// Encapsulates search fields which are available in all full-text search results.
    /// </summary>
    public sealed class SearchFields
    {
        /// <summary>
        /// Gets the title of the search item. Contains data from the field configured as 'Title field' in search index configuration.
        /// </summary>
        public string Title
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the content of the search item. Contains data from the field configured as 'Content field' in search index configuration.
        /// </summary>
        public string Content
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the date associated with search item. Contains data from the field configured as 'Date field' in search index configuration.
        /// If no date information found, contains <see cref="System.DateTime.MinValue"/>. 
        /// </summary>
        public DateTime Date
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the relative image path associated with search item. The image path is based on data from the field configured as 'Image field' in search index configuration.
        /// </summary>
        public string ImagePath
        {
            get;
            internal set;
        }


        /// <summary>
        /// Gets the type of an object where the search item was found in (i.e. cms.document, cms.customtable).
        /// </summary>
        public string ObjectType
        {
            get;
            internal set;
        }
    }
}