using System;
using CMS.DocumentEngine;

namespace Kentico.Search
{
    /// <summary>
    /// Represents one search result.
    /// </summary>
    public class SearchResultItem
    {
        /// <summary>
        /// Image associated with the page. Contains data from field configured as 'Image field' in search index configuration.
        /// </summary>
        public Attachment ImageAttachment
        {
            get;
            set;
        }


        /// <summary>
        /// Title of the page. Contains data from field configured as 'Title field' in search index configuration.
        /// </summary>
        public string Title
        {
            get;
            set;
        }


        /// <summary>
        /// Content of the page. Contains data from field configured as 'Content field' in search index configuration.
        /// </summary>
        public string Content
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the image GUID associated with the search item. Contains data from the field configured as 'Image field' in search index configuration.
        /// </summary>
        public Guid Image
        {
            get;
            internal set;
        }


        /// <summary>
        /// Display name of result's page type.
        /// </summary>
        public string PageTypeDispayName
        {
            get;
            set;
        }


        /// <summary>
        /// Code name of result's page type.
        /// </summary>
        public string PageTypeCodeName
        {
            get;
            set;
        }


        /// <summary>
        /// ID of the result's node.
        /// </summary>
        public int NodeId
        {
            get;
            set;
        }


        /// <summary>
        /// Contains data from field configured as 'Date field' in search index configuration.
        /// </summary>
        public string Date
        {
            get;
            set;
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