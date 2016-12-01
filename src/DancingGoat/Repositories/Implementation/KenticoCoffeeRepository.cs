using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;
using DancingGoat.Infrastructure;
using DancingGoat.Repositories.Filters;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of coffees.
    /// </summary>
    public class KenticoCoffeeRepository : ICoffeeRepository
    {
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoCoffeeRepository"/> class that returns coffees in the specified language.
        /// If the requested coffees doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="siteName">The code name of a site.</param>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoCoffeeRepository(string cultureName, bool latestVersionEnabled)
        {
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns an enumerable collection of coffees ordered by the date of publication.
        /// </summary>
        /// <param name="filter">Instance of a product filter.</param>
        /// <param name="count">The number of coffees to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of coffees.</returns>
        [PagesCacheDependency(Coffee.CLASS_NAME)]
        [CacheDependency("ecommerce.sku|all")]
        public IEnumerable<Coffee> GetCoffees(IRepositoryFilter filter, int count = 0)
        {
            return CoffeeProvider.GetCoffees()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .TopN(count)
                .WhereTrue("SKUEnabled")
                .Where(filter?.GetWhereCondition())
                .OrderByDescending("SKUInStoreFrom")
                .ToList();
        }
    }
}