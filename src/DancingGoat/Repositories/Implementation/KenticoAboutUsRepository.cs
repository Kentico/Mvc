using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of stories about company's strategy, history and philosophy.
    /// </summary>
    public class KenticoAboutUsRepository : IAboutUsRepository
    {
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoAboutUsRepository"/> class that returns stories in the specified language. 
        /// If the requested story doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoAboutUsRepository(string cultureName, bool latestVersionEnabled)
        {
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns an enumerable collection of stories about company's philosophy ordered by a position in the content tree.
        /// </summary>
        /// <returns>An enumerable collection of stories about company's philosophy ordered by a position in the content tree.</returns>
        public IEnumerable<AboutUsSection> GetSideStories()
        {
            return AboutUsSectionProvider.GetAboutUsSections()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(SiteContext.CurrentSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder")
                .ToList();
        }
    }
}