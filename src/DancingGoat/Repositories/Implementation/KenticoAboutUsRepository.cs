using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of stories about company's strategy, history and philosophy.
    /// </summary>
    public class KenticoAboutUsRepository : IAboutUsRepository
    {
        private readonly string mSiteName;
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoAboutUsRepository"/> class that returns stories from the specified site in the specified language. 
        /// If the requested story doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="siteName">The code name of a site.</param>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoAboutUsRepository(string siteName, string cultureName, bool latestVersionEnabled)
        {
            mSiteName = siteName;
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }

        
        /// <summary>
        /// Returns the story that describes company's strategy and history.
        /// </summary>
        /// <returns>The story that describes company's strategy and history, if found; otherwise, null.</returns>
        public AboutUs GetOurStory()
        {
            return AboutUsProvider.GetAboutUs()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(mSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .TopN(1);
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
                .OnSite(mSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .OrderBy("NodeOrder")
                .ToList();
        }
    }
}