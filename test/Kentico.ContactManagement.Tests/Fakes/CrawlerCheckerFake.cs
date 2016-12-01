using CMS.Helpers.Internal;

namespace Kentico.ContactManagement.Tests
{
    public class CrawlerCheckerFake : ICrawlerChecker
    {
        public bool Crawler
        {
            set;
            get;
        }


        public CrawlerCheckerFake()
        {
            Crawler = false;
        }


        /// <summary>
        /// Checks whether the current request comes from the crawler.
        /// </summary>
        /// <returns>
        /// False if not changed through <see cref="CrawlerCheckerFake.Crawler"/> property.
        /// </returns>
        public bool IsCrawler()
        {
            return Crawler;
        }
    }
}
