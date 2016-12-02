using CMS.Activities;

namespace Kentico.Activities
{
    /// <summary>
    /// Provides possibility to log E-commerce activities. This class should be always used instead of <see cref="CMS.Ecommerce.EcommerceActivityLogger"/> to make sure
    /// proper implementation of <see cref="IActivityUrlService"/> is used.
    /// </summary>
    public class EcommerceActivityLogger : CMS.Ecommerce.EcommerceActivityLogger
    {
    }
}