using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Repositories.Filters;

using Kentico.Core.DependencyInjection;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of coffees.
    /// </summary>
    public interface ICoffeeRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of coffees ordered by the date of publication.
        /// </summary>
        /// <param name="filter">Repository filter.</param>
        /// <param name="count">The number of coffees to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of coffees.</returns>
        IEnumerable<Coffee> GetCoffees(IRepositoryFilter filter, int count = 0);
    }
}