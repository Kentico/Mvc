using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of home page sections.
    /// </summary>
    public interface IHomeRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of home page sections ordered by a position in the content tree.
        /// </summary>
        IEnumerable<HomeSection> GetHomeSections();
    }
}