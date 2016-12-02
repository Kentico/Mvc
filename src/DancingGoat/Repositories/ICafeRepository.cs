using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of cafes.
    /// </summary>
    public interface ICafeRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of company cafes ordered by a position in the content tree.
        /// </summary>
        /// <param name="count">The number of cafes to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of cafes ordered by a position in the content tree.</returns>
        IEnumerable<Cafe> GetCompanyCafes(int count = 0);


        /// <summary>
        /// Returns an enumerable collection of partner cafes ordered by a position in the content tree.
        /// </summary>
        /// <returns>An enumerable collection of partner cafes ordered by a position in the content tree.</returns>
        IEnumerable<Cafe> GetPartnerCafes();
    }
}