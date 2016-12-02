using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of articles.
    /// </summary>
    public class KenticoArticleRepository : IArticleRepository
    {
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoArticleRepository"/> class that returns articles in the specified language.
        /// If the requested article doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoArticleRepository(string cultureName, bool latestVersionEnabled)
        {
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns an enumerable collection of articles ordered by the date of publication. The most recent articles come first.
        /// </summary>
        /// <param name="count">The number of articles to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of articles ordered by the date of publication.</returns>
        public IEnumerable<Article> GetArticles(int count = 0)
        {
            return ArticleProvider.GetArticles()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .TopN(count)
                .OrderByDescending("DocumentPublishFrom")
                .ToList();
        }


        /// <summary>
        /// Returns the article with the specified identifier.
        /// </summary>
        /// <param name="nodeID">The article node identifier.</param>
        /// <returns>The article with the specified node identifier, if found; otherwise, null.</returns>
        public Article GetArticle(int nodeID)
        {
            return ArticleProvider.GetArticle(nodeID, mCultureName, SiteContext.CurrentSiteName)
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .CombineWithDefaultCulture();
        }
    }
}