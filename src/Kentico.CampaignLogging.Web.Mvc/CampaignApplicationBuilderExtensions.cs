using System.Web.Mvc;

using Kentico.Web.Mvc;

namespace Kentico.CampaignLogging.Web.Mvc
{
    /// <summary>
    /// Provides methods for logging campaign details after customer visits some URL.
    /// </summary>
    public static class CampaignApplicationBuilderExtensions
    {
        /// <summary>
        /// Enables the campaign logging of UTM parameters for each request.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static void UseCampaignLogger(this ApplicationBuilder builder)
        {
            GlobalFilters.Filters.Add(new CampaignLoggerFilter());
        }
    }
}
