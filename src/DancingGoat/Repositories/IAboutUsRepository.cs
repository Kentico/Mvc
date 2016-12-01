using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of stories about company's strategy, history and philosophy.
    /// </summary>
    public interface IAboutUsRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of stories about company's philosophy ordered by a position in the content tree.
        /// </summary>
        /// <returns>An enumerable collection of stories about company's philosophy ordered by a position in the content tree.</returns>
        IEnumerable<AboutUsSection> GetSideStories();
    }
}