using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.WebAnalytics;

namespace Kentico.Search
{
    /// <summary>
    /// Provides access to Kentico Smart search.
    /// </summary>
    public class SearchService : ISearchService
    {
        #region "Variables"

        internal DataSet rawResults;
        private int totalNumberOfResults;

        /// <summary>
        /// Activity logger used to log internal search activity.
        /// </summary>
        protected PagesActivityLogger mPagesActivityLogger = new PagesActivityLogger();

        #endregion


        #region "Public methods"

        /// <summary>
        /// Performs full-text search for the requested page.
        /// </summary>
        /// <param name="options">Options that specify what should be searched for.</param>
        /// <returns>
        /// Search result for the specified search <paramref name="options" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        public virtual SearchResult Search(SearchOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // Display first page for invalid page numbers
            options.PageNumber = Math.Max(options.PageNumber, 1);

            PerformSearch(options);
            LogInternalSearchActivity(options.Query);

            return new SearchResult
            {
                Items = GetSearchItems(),
                TotalNumberOfResults = totalNumberOfResults
            };
        }


        #endregion


        #region "Private Methods"


        /// <summary>
        /// Logs activity internal search for given search query.
        /// </summary>
        private void LogInternalSearchActivity(string searchKeyword)
        {
            mPagesActivityLogger.LogInternalSearch(searchKeyword);
        }


        /// <summary>
        /// Performs search and populates the returned raw data into the internal search service dataset. Logs activity.
        /// </summary>
        private void PerformSearch(SearchOptions options)
        {
            var documentCondition = new DocumentSearchCondition(null, options.CultureName, options.DefaultCultureCode, options.CombineWithDefaultCulture);
            var condition = new SearchCondition(documentCondition: documentCondition);
            var searchExpression = SearchSyntaxHelper.CombineSearchCondition(options.Query, condition);

            var parameters = new SearchParameters
            {
                SearchFor = searchExpression,
                Path = "/%",
                ClassNames = null,
                CurrentCulture = options.CultureName,
                DefaultCulture = options.DefaultCultureCode,
                CombineWithDefaultCulture = options.CombineWithDefaultCulture,
                CheckPermissions = false,
                SearchInAttachments = false,
                User = MembershipContext.AuthenticatedUser,
                SearchIndexes = options.SearchIndexNames.Join(";"),
                StartingPosition = (options.PageNumber - 1) * options.PageSize,
                NumberOfResults = 0,
                NumberOfProcessedResults = 100,
                DisplayResults = options.PageSize
            };

            // Search and save results
            rawResults = SearchInternal(parameters);
            totalNumberOfResults = parameters.NumberOfResults;
        }


        /// <summary>
        /// Returns dataset with search results using Kentico Smart search.
        /// </summary>
        internal virtual DataSet SearchInternal(SearchParameters parameters)
        {
            return SearchHelper.Search(parameters);
        }


        private IEnumerable<SearchResultItem<BaseInfo>> GetSearchItems()
        {
            if (DataHelper.DataSourceIsEmpty(SearchContext.CurrentSearchResults) || (rawResults == null) || (SearchContext.CurrentSearchResults == null))
            {
                return Enumerable.Empty<SearchResultItem<BaseInfo>>();
            }

            var searchItems = new List<SearchResultItem<BaseInfo>>();
            foreach (DataRow row in rawResults.Tables[0].Rows)
            {
                var searchItem = CreateSearchResultItem(row);
                searchItems.Add(searchItem);
            }

            return searchItems;
        }


        private SearchResultItem<BaseInfo> CreateSearchResultItem(DataRow row)
        {
            DateTime date;
            DateTime.TryParse(row["created"].ToString(), out date);
            var objectType = row["type"].ToString();
            var id = row["id"].ToString();

            var searchItem = new SearchResultItem<BaseInfo>
            {
                Fields = new SearchFields
                {
                    Title = row["title"].ToString(),
                    Content = row["content"].ToString(),
                    ImagePath = GetImagePath(objectType, id, row["image"].ToString()),
                    Date = date,
                    ObjectType = objectType
                },
                Data = GetDataObject(id, objectType)
            };

            return searchItem;
        }


        internal virtual string GetImagePath(string objectType, string id, string image)
        {
            return SearchIndexers.GetIndexer(objectType)?.GetSearchImageUrl(id, objectType, image);
        }


        internal virtual BaseInfo GetDataObject(string id, string objectType)
        {
            var dataRow = GetDataFromSearchContext(id);
            return ModuleManager.GetObject(new LoadDataSettings(dataRow, objectType)); 
        }


        private static DataRow GetDataFromSearchContext(string searchId)
        {
            return SearchContext.CurrentSearchResults[searchId];
        }

        #endregion
    }
}