using System;

using CMS.Core;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.WebAnalytics;

using Kentico.CampaignLogging;
using Kentico.VisitorActionLogging;

[assembly: VisitorActionLogging(typeof(CampaignLogger))]

namespace Kentico.CampaignLogging
{
    /// <summary>
    /// Provides methods for logging campaign details after customer he visits some URL.
    /// </summary>
    public class CampaignLogger : IVisitorActionLogger
    {
        private const string UTM_CAMPAIGN_PARAMETER_NAME = "utm_campaign";
        private const string UTM_SOURCE_PARAMETER_NAME = "utm_source";

        private readonly ICampaignService mCampaignService;


        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignLogger"/>.
        /// </summary>
        public CampaignLogger()
        {
            mCampaignService = Service.Entry<ICampaignService>();
        }


        /// <summary>
        /// Sets campaign service, UTM campaign code and source from query string.
        /// </summary>
        /// <param name="title">Page title.</param>
        /// <param name="url">Current URL in browser.</param>
        /// <param name="referrer">HTTP referrer, URL of the previous from which a link was followed.</param>
        public void Log(string title, string url, string referrer)
        {
            var utmCode = URLHelper.GetQueryValue(url, UTM_CAMPAIGN_PARAMETER_NAME);
            if (!String.IsNullOrEmpty(utmCode))
            {
                var source = URLHelper.GetQueryValue(url, UTM_SOURCE_PARAMETER_NAME);
                mCampaignService.SetCampaign(utmCode, SiteContext.CurrentSiteName, source);
            }
        }
    }
}
