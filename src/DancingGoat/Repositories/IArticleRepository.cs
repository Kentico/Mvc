using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of articles.
    /// </summary>
    public interface IArticleRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of articles ordered by the date of publication. The most recent articles come first.
        /// </summary>
        /// <param name="count">The number of articles to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of articles ordered by the date of publication.</returns>
        IEnumerable<Article> GetArticles(int count = 0);


        /// <summary>
        /// Returns the article with the specified identifier.
        /// </summary>
        /// <param name="nodeID">The article identifier.</param>
        /// <returns>The article with the specified node identifier, if found; otherwise, null.</returns>
        Article GetArticle(int nodeID);
    }
}