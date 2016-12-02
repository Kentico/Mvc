using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;
using DancingGoat.Infrastructure;
using DancingGoat.Repositories.Filters;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of brewers.
    /// </summary>
    public class KenticoBrewerRepository : IBrewerRepository
    {
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoBrewerRepository"/> class that returns brewers in the specified language.
        /// If the requested brewers doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoBrewerRepository(string cultureName, bool latestVersionEnabled)
        {
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns an enumerable collection of brewers ordered by the date of publication.
        /// </summary>
        /// <param name="filter">Repository filter.</param>
        /// <param name="count">The number of brewers to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of brewers.</returns>
        [PagesCacheDependency(Brewer.CLASS_NAME)]
        [CacheDependency("ecommerce.sku|all")]
        public IEnumerable<Brewer> GetBrewers(IRepositoryFilter filter, int count = 0)
        {
            return BrewerProvider.GetBrewers()
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