using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a contract for different promoted content. The best, coolest and newest content from all.
    /// </summary>
    public class PromotedContentRepository : IPromotedContentRepository
    {
        private readonly IArticleRepository mArticlesRepository;
        private readonly ICafeRepository mCafeRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="PromotedContentRepository"/> class that provides newest and most interesting content.
        /// </summary>
        /// <param name="articlesRepository">Articles repository used to obtain newest articles.</param>
        /// <param name="cafeRepository">Cafes repository used to obtain promoted cafes.</param>
        public PromotedContentRepository(IArticleRepository articlesRepository, ICafeRepository cafeRepository)
        {
            mArticlesRepository = articlesRepository;
            mCafeRepository = cafeRepository;
        }


        /// <summary>
        /// Returns an enumerable collection of articles ordered by the date of publication. The most recent articles come first.
        /// </summary>
        /// <param name="count">The number of articles to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of articles ordered by the date of publication.</returns>
        public IEnumerable<Article> GetNewestArticles(int count = 0)
        {
            return mArticlesRepository.GetArticles(count);
        }


        /// <summary>
        /// Returns an enumerable collection of company cafes ordered by a significance.
        /// </summary>
        /// <param name="count">The number of cafes to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of cafes ordered by a significance.</returns>
        public IEnumerable<Cafe> GetPromotedCompanyCafes(int count = 0)
        {
            return mCafeRepository.GetCompanyCafes(count);
        }
    }
}