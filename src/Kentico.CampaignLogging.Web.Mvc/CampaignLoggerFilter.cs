using System.Web;
using System.Web.Mvc;

using CMS.Core;
using CMS.SiteProvider;
using CMS.WebAnalytics;

namespace Kentico.CampaignLogging.Web.Mvc
{
    /// <summary>
    /// Handles logging of campaign details for visitors.
    /// </summary>
    public class CampaignLoggerFilter : ActionFilterAttribute
    {
        private const string UTM_CAMPAIGN_PARAMETER_NAME = "utm_campaign";
        private const string UTM_SOURCE_PARAMETER_NAME = "utm_source";

        private readonly ICampaignService mCampaignService;


        /// <summary>
        /// Creates a new instance of <see cref="CampaignLoggerFilter"/> for campaign logging.
        /// </summary>
        public CampaignLoggerFilter()
        {
            mCampaignService = Service.Resolve<ICampaignService>();
        }


        /// <summary>
        /// Stores the campaign UTM parameters of current url using the <see cref="ICampaignService"/>.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                // Prevent from processing multiple times
                return;
            }

            var req = filterContext.HttpContext.Request;
            var utmCode = req.QueryString[UTM_CAMPAIGN_PARAMETER_NAME];

            if (!string.IsNullOrEmpty(utmCode))
            {
                var source = req.QueryString[UTM_SOURCE_PARAMETER_NAME];
                mCampaignService.SetCampaign(utmCode, SiteContext.CurrentSiteName, source);
            }

            // Attach cache validation to check UTM params before the page is returned from cache
            HttpContext.Current.Response.Cache.AddValidationCallback(CheckUTMParams, null);
        }


        /// <summary>
        /// Cached result is not used to ensure campaign cookies in case of presence of UTM campaign in the query string.
        /// </summary>
        private void CheckUTMParams(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString[UTM_CAMPAIGN_PARAMETER_NAME]))
            {
                validationStatus = HttpValidationStatus.IgnoreThisRequest;
            }
        }
    }
}
