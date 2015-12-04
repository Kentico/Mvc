using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;

namespace Kentico.Search
{
    /// <summary>
    /// Provides access to Kentico smart search.
    /// </summary>
    public class SearchService
    {
        #region "Variables"

        private DataSet mRawResults;
        private readonly string mCultureName;
        private readonly string[] mSearchIndexNames;
        private readonly string mDefaultCulture;
        private readonly string mSiteName;
        private readonly bool mCombineWithDefaultCulture;

        #endregion


        #region "Public Methods"

        /// <summary>
        /// Creates new instance of <see cref="SearchService"/> searching in specified search index for given site and culture.
        /// </summary>
        /// <param name="searchIndexNames">Array of search index names to search in</param>
        /// <param name="cultureName">Culture name to search in. If null, searches in all cultures.</param>
        /// <param name="siteName">Site name</param>
        /// <param name="combineWithDefaultCulture">Indicates whether the search service uses default language version of pages as a replacement for pages that are not translated into the specified language.</param>
        /// <exception cref="ArgumentNullException"><paramref name="searchIndexNames"/> is null.</exception>
        public SearchService(string[] searchIndexNames, string cultureName, string siteName, bool combineWithDefaultCulture)
        {
            if (searchIndexNames == null)
            {
                throw new ArgumentNullException("searchIndexNames");
            }

            mCultureName = cultureName;
            mSearchIndexNames = searchIndexNames;
            mDefaultCulture = CultureHelper.GetDefaultCultureCode(siteName);
            mSiteName = siteName;
            mCombineWithDefaultCulture = combineWithDefaultCulture;
        }


        /// <summary>
        /// Performs full-text search.
        /// </summary>
        /// <param name="query">Text to search</param>
        /// <param name="page">Zero-based page index</param>
        /// <param name="pageSize">Specifies the number of results per page</param>
        /// <param name="numberOfResults">Total number of search results</param>
        /// <returns>Search results or null if <paramref name="query"/> is empty.</returns>
        public virtual IEnumerable<SearchResultItem> Search(string query, int page, int pageSize, out int numberOfResults)
        {
            if (String.IsNullOrWhiteSpace(query))
            {
                numberOfResults = 0;
                return null;
            }

            SearchInternal(query, page, pageSize, out numberOfResults);
            return GetSearchItems();
        }

        #endregion


        #region "Private Methods"

        /// <summary>
        /// Performs search and populates the returned raw data into the internal search service dataset.
        /// </summary>
        /// <param name="query">Text to search</param>
        /// <param name="pageIndex">Zero-based page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="numberOfResults">Total number of search results</param>
        private void SearchInternal(string query, int pageIndex, int pageSize, out int numberOfResults)
        {
            var documentCondition = new DocumentSearchCondition(null, mCultureName, mDefaultCulture, mCombineWithDefaultCulture);
            var condition = new SearchCondition(documentCondition: documentCondition);
            var searchExpression = SearchSyntaxHelper.CombineSearchCondition(query, condition);

            var parameters = new SearchParameters
            {
                SearchFor = searchExpression,
                Path = "/%",
                ClassNames = null,
                CurrentCulture = mCultureName,
                DefaultCulture = mDefaultCulture,
                CombineWithDefaultCulture = mCombineWithDefaultCulture,
                CheckPermissions = false,
                SearchInAttachments = false,
                User = MembershipContext.AuthenticatedUser,
                SearchIndexes = mSearchIndexNames.Join(";"),
                StartingPosition = pageIndex * pageSize,
                NumberOfResults = 0,
                NumberOfProcessedResults = 100,
                DisplayResults = pageSize
            };

            // Search and save results
            mRawResults = SearchHelper.Search(parameters);
            numberOfResults = parameters.NumberOfResults;
        }


        /// <summary>
        /// Gets search items filled with all necessary properties from <see cref="SearchContext.CurrentSearchResults"/> and <see cref="mRawResults"/> collection.
        /// </summary>
        /// <returns>Search items collection</returns>
        private IEnumerable<SearchResultItem> GetSearchItems()
        {
            var searchItems = new List<SearchResultItem>();

            if (DataHelper.DataSourceIsEmpty(SearchContext.CurrentSearchResults) || (mRawResults == null) || (SearchContext.CurrentSearchResults == null))
            {
                return null;
            }

            var attachmentIdentifiers = new List<Guid>();
            foreach (DataRow row in mRawResults.Tables[0].Rows)
            {
                string date, pageTypeDisplayName, pageTypeCodeName;
                int nodeId;
                int documentNodeId = GetDocumentNodeId(row["type"], row["id"]);
                Guid imageGuid = ((row["image"] as string) == null) ? Guid.Empty : new Guid(row["image"].ToString());
                attachmentIdentifiers.Add(imageGuid);

                GetAdditionalData(row["type"], documentNodeId, out date, out nodeId, out pageTypeDisplayName, out pageTypeCodeName);

                var searchItem = new SearchResultItem
                {
                    NodeId = nodeId,
                    Title = row["title"].ToString(),
                    Content = row["content"].ToString(),
                    Date = date,
                    PageTypeDispayName = pageTypeDisplayName,
                    PageTypeCodeName = pageTypeCodeName
                };

                searchItems.Add(searchItem);
            }

            var attachments = AttachmentInfoProvider.GetAttachments().OnSite(mSiteName).BinaryData(false).WhereIn("AttachmentGUID", attachmentIdentifiers).ToDictionary(x => x.AttachmentGUID);
            for (int i = 0; i < searchItems.Count; i++)
            {
                AttachmentInfo attachment;
                if (attachments.TryGetValue(attachmentIdentifiers[i], out attachment))
                {
                    searchItems[i].ImageAttachment = new Attachment(attachment);
                }
            }

            return searchItems;
        }


        /// <summary>
        /// Gets additional data from <see cref="SearchContext.CurrentSearchResults"/> collection.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="documentNodeId">DocumentNodeID</param>
        /// <param name="date">Last modified date</param>
        /// <param name="nodeId">NodeID</param>
        /// <param name="pageTypeDisplayName">Page type display name</param>
        /// <param name="pageTypeCodeName">Page type code name</param>
        private static void GetAdditionalData(object type, int documentNodeId, out string date, out int nodeId, out string pageTypeDisplayName, out string pageTypeCodeName)
        {
            foreach (var key in SearchContext.CurrentSearchResults.Keys)
            {
                if (GetDocumentNodeId(type, key) == documentNodeId)
                {
                    var row = (DataRow)SearchContext.CurrentSearchResults[key];

                    date = row["DocumentModifiedWhen"].ToString();
                    nodeId = ValidationHelper.GetInteger(row["NodeID"], 0);
                    pageTypeDisplayName = row["ClassDisplayName"].ToString();
                    pageTypeCodeName = row["ClassName"].ToString();

                    return;
                }
            }

            date = pageTypeDisplayName = pageTypeCodeName = "";
            nodeId = 0;
        }


        /// <summary>
        /// Parses the key and extracts DocumentNodeID depending on Type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <returns>Returns DocumentNodeID</returns>
        private static int GetDocumentNodeId(object type, object key)
        {
            string stringType = type.ToString();
            string stringKey = key.ToString();

            int i = stringKey.IndexOf(';') + 1;
            int j = stringKey.IndexOf(stringType, StringComparison.Ordinal) - 1;

            return ValidationHelper.GetInteger(stringKey.Substring(i, j - i), 0);
        }

        #endregion
    }
}