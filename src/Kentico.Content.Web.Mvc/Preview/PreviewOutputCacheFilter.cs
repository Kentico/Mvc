using System.Web;
using System.Web.Mvc;

using Kentico.Web.Mvc;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Adds validation callback for the output cache item to ignore it in a preview mode.
    /// </summary>
    internal class PreviewOutputCacheFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;

            if (!context.Kentico().Preview().Enabled)
            {
                context.Response.Cache.AddValidationCallback(new HttpCacheValidateHandler(ValidateCacheItem), null);
            }
        }


        private static void ValidateCacheItem(HttpContext context, object data, ref HttpValidationStatus status)
        {
            if (context.Kentico().Preview().Enabled)
            {
                status = HttpValidationStatus.IgnoreThisRequest;
            }
        }
    }
}
