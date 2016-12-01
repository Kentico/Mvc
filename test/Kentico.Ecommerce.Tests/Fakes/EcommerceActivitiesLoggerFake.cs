using System.Web;

using Kentico.Activities;

namespace Kentico.Ecommerce.Tests.Fakes
{
    internal class EcommerceActivitiesLoggerFake : EcommerceActivityLogger
    {
        protected override HttpRequestBase GetCurrentRequest()
        {
            return null;
        }
    }
}