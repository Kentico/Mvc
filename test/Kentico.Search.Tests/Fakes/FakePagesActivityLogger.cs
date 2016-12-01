using System.Web;

using CMS.WebAnalytics;

namespace Kentico.Search.Tests
{
    public class FakePagesActivityLogger : PagesActivityLogger
    {
        protected override HttpRequestBase GetCurrentRequest()
        {
            return null;
        }
    }
}