using System;
using System.Collections.Generic;
using System.Globalization;
using CMS.Helpers;
using CMS.SiteProvider;

namespace Kentico.Search
{
    /// <summary>
    /// Represents options that define what the Kentico search service will search for.
    /// </summary>
    public sealed class SearchOptions
    {
        /// <summary>
        /// Gets or sets name of culture to search in. If null, searches in all cultures.
        /// The default value is <see cref="CultureInfo.Name"/> of <see cref="CultureInfo.CurrentUICulture"/>.
        /// </summary>
        public string CultureName
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets code of culture to be used as a default.
        /// The default value is retrieved via <see cref="CultureHelper.GetDefaultCultureCode"/> call for <see cref="SiteContext.CurrentSiteName"/>.
        /// </summary>
        internal string DefaultCultureCode
        {
            get;
            set;
        }


        /// <summary>
        /// Indicates whether the search service uses site default language version of pages as a replacement for pages that are not translated into the language specified by <see cref="CultureName"/>.
        /// The site default language is retrieved via <see cref="CultureHelper.GetDefaultCultureCode"/> call for <see cref="SiteContext.CurrentSiteName"/>
        /// </summary>
        public bool CombineWithDefaultCulture
        {
            get;
            set;    
        }


        /// <summary>
        /// Gets the text to search.
        /// </summary>
        public string Query
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the one-based page number. Page number must be a positive number.
        /// </summary>
        public int PageNumber
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the size of the page. Page size must be a positive number.
        /// </summary>
        public int PageSize
        {
            get;
            set;
        }


        /// <summary>
        /// Gets enumeration of search index code names to search in.
        /// </summary>
        public IEnumerable<string> SearchIndexNames
        {
            get;
            private set;
        }


        /// <summary>
        /// Initializes default search options.
        /// </summary>
        /// <param name="query">Text to search.</param>
        /// <param name="searchIndexNames">Enumeration of search index code names to search in.</param>
        /// <exception cref="ArgumentNullException"><paramref name="searchIndexNames"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is null or whitespace string.</exception>
        public SearchOptions(string query, IEnumerable<string> searchIndexNames)
        {
            if (String.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Query cannot be null or whitespace.", nameof(query));
            }
            if (searchIndexNames == null)
            {
                throw new ArgumentNullException(nameof(searchIndexNames));
            }

            CultureName = CultureInfo.CurrentUICulture.Name;
            DefaultCultureCode = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
            Query = query;
            SearchIndexNames = searchIndexNames;
        }
    }
}
