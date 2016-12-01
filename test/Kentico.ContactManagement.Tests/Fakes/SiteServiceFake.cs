using CMS.Base;
using CMS.SiteProvider;

using NSubstitute;

namespace Kentico.ContactManagement.Tests
{
    public class SiteServiceFake : ISiteService
    {
        private static readonly string SITE_NAME = "TestSite";


        public SiteServiceFake()
        {
            var site = Substitute.For<SiteInfo>();
            site.SiteName.Returns(SITE_NAME);

            CurrentSite = site;
            IsLiveSite = false;
        }

        /// <summary>
        /// Current substituted site.
        /// </summary>
        public ISiteInfo CurrentSite { get; }

        /// <summary>
        /// Returns false for this fake.
        /// </summary>
        /// <returns>Always false</returns>
        public bool IsLiveSite { get; }
    }
}
