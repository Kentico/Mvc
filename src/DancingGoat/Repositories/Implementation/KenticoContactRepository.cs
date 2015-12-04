using System;
using System.Linq;

using CMS.DocumentEngine.Types;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of contact information.
    /// </summary>
    public class KenticoContactRepository : IContactRepository
    {
        private readonly string mSiteName;
        private readonly string mCultureName;
        private readonly bool mLatestVersionEnabled;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoContactRepository"/> class that returns contact information from the specified site using the specified language.
        /// If the requested contact doesn't exist in specified language then its default culture version is returned.
        /// </summary>
        /// <param name="siteName">The code name of a site.</param>
        /// <param name="cultureName">The name of a culture.</param>
        /// <param name="latestVersionEnabled">Indicates whether the repository will provide the most recent version of pages.</param>
        public KenticoContactRepository(string siteName, string cultureName, bool latestVersionEnabled)
        {
            mSiteName = siteName;
            mCultureName = cultureName;
            mLatestVersionEnabled = latestVersionEnabled;
        }


        /// <summary>
        /// Returns company's contact information.
        /// </summary>
        /// <returns>Company's contact information, if found; otherwise, null.</returns>
        public Contact GetCompanyContact()
        {
            return ContactProvider.GetContacts()
                .LatestVersion(mLatestVersionEnabled)
                .Published(!mLatestVersionEnabled)
                .OnSite(mSiteName)
                .Culture(mCultureName)
                .CombineWithDefaultCulture()
                .TopN(1);
        }
    }
}