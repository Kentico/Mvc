using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for different promoted content. The best, coolest and newest content from all.
    /// </summary>
    public interface IPromotedContentRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of articles ordered by the date of publication. The most recent articles come first.
        /// </summary>
        /// <param name="count">The number of articles to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of articles ordered by the date of publication.</returns>
        IEnumerable<Article> GetNewestArticles(int count = 0);


        /// <summary>
        /// Returns an enumerable collection of company cafes ordered by a significance.
        /// </summary>
        /// <param name="count">The number of cafes to return. Use 0 as value to return all records.param>
        /// <returns>An enumerable collection that contains the specified number of cafes ordered by a significance.</returns>
        IEnumerable<Cafe> GetPromotedCompanyCafes(int count = 0);
    }
}