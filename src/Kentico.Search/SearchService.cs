using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;

namespace Kentico.Search
{
    /// <summary>
    /// Provides access to Kentico Smart search.
    /// </summary>
    public class SearchService
    {
        #region "Variables"

        internal DataSet mRawResults;
        private readonly string mCultureName;
        private readonly string[] mSearchIndexNames;
        private readonly string mDefaultCulture;
        private readonly string mSiteName;
        private readonly bool mCombineWithDefaultCulture;

        #endregion


        #region "Public Methods"

        /// <summary>
        /// Creates new instance of <see cref="SearchService"/> searching in specified search indexes for given site and culture.
        /// </summary>
        /// <param name="searchIndexNames">Array of search index code names to search in.</param>
        /// <param name="cultureName">Culture name to search in. If null, searches in all cultures.</param>
        /// <param name="siteName">Site code name.</param>
        /// <param name="combineWithDefaultCulture">Indicates whether the search service uses site default language version of pages as a replacement for pages that are not translated into the specified language.</param>
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
        /// Performs paged full-text search.
        /// </summary>
        /// <param name="query">Text to search.</param>
        /// <param name="page">Zero-based page index.</param>
        /// <param name="pageSize">Specifies the number of results per page. Page size must be a positive number.</param>
        /// <param name="numberOfResults">Total number of search results for the given query which can be retrieved from the search index.</param>
        /// <returns>Search results for the specified page or null if <paramref name="query"/> is empty.</returns>
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


        #region "Private methods"

        /// <summary>
        /// Performs search and populates the returned raw data into the internal search service dataset.
        /// </summary>
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
            mRawResults = Search(parameters);
            numberOfResults = parameters.NumberOfResults;
        }


        /// <summary>
        /// Returns dataset with search results using Kentico Smart search. If search is used for non-page index, path and class name values are ignored (can be null).
        /// </summary>
        internal virtual DataSet Search(SearchParameters parameters)
        {
            return SearchHelper.Search(parameters);
        }


        /// <summary>
        /// Gets search items collection filled with all necessary properties from <see cref="SearchContext.CurrentSearchResults"/> and <see cref="mRawResults"/> collection.
        /// </summary>
        /// <returns>Search items collection</returns>
        private IEnumerable<SearchResultItem> GetSearchItems()
        {
            var searchItems = new List<SearchResultItem>();

            if (DataHelper.DataSourceIsEmpty(SearchContext.CurrentSearchResults) || (mRawResults == null) || (SearchContext.CurrentSearchResults == null))
            {
                return null;
            }

            var pageAttachmentGUIDs = new List<Guid>();
            foreach (DataRow row in mRawResults.Tables[0].Rows)
            {
                string pageTypeDisplayName = String.Empty;
                string pageTypeCodeName = String.Empty;
                int nodeId = 0;
                string date = row["created"].ToString();
                Guid imageGuid = ((row["image"] as string) == null) ? Guid.Empty : new Guid(row["image"].ToString());
                string objectType = row["type"].ToString();


                if (objectType == PredefinedObjectType.DOCUMENT)
                {
                    // Get additional info specific for page objects
                    int documentNodeId = GetDocumentNodeId(objectType, row["id"].ToString());
                    GetAdditionalData(objectType, documentNodeId, out nodeId, out pageTypeDisplayName, out pageTypeCodeName);

                    if (imageGuid != Guid.Empty)
                    {
                        pageAttachmentGUIDs.Add(imageGuid);
                    }
                }

                var searchItem = new SearchResultItem
                {
                    // General object info
                    Title = row["title"].ToString(),
                    Content = row["content"].ToString(),
                    Image = imageGuid,
                    Date = date,
                    ObjectType = objectType,

                    // Page related info
                    NodeId = nodeId,
                    PageTypeDispayName = pageTypeDisplayName,
                    PageTypeCodeName = pageTypeCodeName
                };

                searchItems.Add(searchItem);
            }

            // Add page attachments to the searchItems collection
            if (pageAttachmentGUIDs.Count > 0)
            {
                var attachments = GetPageAttachments(pageAttachmentGUIDs);
                foreach (var item in searchItems)
                {
                    if (item.Image != Guid.Empty)
                    {
                        AttachmentInfo attachmentInfo;
                        if (attachments.TryGetValue(item.Image, out attachmentInfo))
                        {
                            item.ImageAttachment = new Attachment(attachmentInfo);
                        }
                    }
                }
            }

            return searchItems;
        }


        /// <summary>
        /// Gets additional data from <see cref="SearchContext.CurrentSearchResults"/> collection.
        /// </summary>
        private static void GetAdditionalData(string type, int documentNodeId, out int nodeId, out string pageTypeDisplayName, out string pageTypeCodeName)
        {
            foreach (string key in SearchContext.CurrentSearchResults.Keys)
            {
                if (GetDocumentNodeId(type, key) == documentNodeId)
                {
                    var row = (DataRow)SearchContext.CurrentSearchResults[key];

                    nodeId = ValidationHelper.GetInteger(row["NodeID"], 0);
                    pageTypeDisplayName = row["ClassDisplayName"].ToString();
                    pageTypeCodeName = row["ClassName"].ToString();

                    return;
                }
            }

            pageTypeDisplayName = pageTypeCodeName = String.Empty;
            nodeId = 0;
        }


        /// <summary>
        /// Parses the key and extracts DocumentNodeID depending on Type.
        /// </summary>
        /// <returns>Returns DocumentNodeID</returns>
        private static int GetDocumentNodeId(string objectType, string key)
        {
            int i = key.IndexOf(';') + 1;
            int j = key.IndexOf(objectType, StringComparison.Ordinal) - 1;

            return ValidationHelper.GetInteger(key.Substring(i, j - i), 0);
        }


        /// <summary>
        /// Gets the page attachment info objects for the given attachment GUIDs.
        /// </summary>
        internal virtual Dictionary<Guid, AttachmentInfo> GetPageAttachments(ICollection<Guid> attachmentGuids)
        {
            return AttachmentInfoProvider.GetAttachments().OnSite(mSiteName).BinaryData(false).WhereIn("AttachmentGUID", attachmentGuids).ToDictionary(x => x.AttachmentGUID);
        }

        #endregion
    }
}