using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CMS.SiteProvider;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Module enabling cross origin resource sharing (CORS) with domains bound to current site.
    /// When request is sent from current site's domain, adds Access-Control-Allow-Origin header into response to enable CORS.
    /// </summary>
    internal class CrossOriginResourceSharingWithCurrentSiteModule : IModule
    {
        private const string ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME = "Access-Control-Allow-Origin";
        private const string ORIGIN_HEADER_NAME = "Origin";


        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="application">The ASP.NET application that defines common methods, properties, and events.</param>
        /// <exception cref="ArgumentNullException"><paramref name="application"/> is null.</exception>
        public void Initialize(HttpApplication application)
        {
            application.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }


        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            SetHeaderToAllowCurrentSiteOrigin(httpContext);
        }


        /// <summary>
        /// Sets response header Access-Control-Allow-Origin if request Origin is current site
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        internal static void SetHeaderToAllowCurrentSiteOrigin(HttpContextBase httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            string requestOrigin = request.Headers[ORIGIN_HEADER_NAME];

            if (IsCurrentSiteOrigin(requestOrigin))
            {
                SetAccessControlAllowOriginHeader(response, requestOrigin);
            }
        }


        /// <summary>
        /// Finds out if the <paramref name="requestOrigin"/> (URI) belongs to current site's administration
        /// so the request origin can be considered as trusted.
        /// </summary>
        /// <param name="requestOrigin">URI of the request origin</param>
        /// <returns>True, if the requestHeaders origin domain is one of current site's domains</returns>
        private static bool IsCurrentSiteOrigin(string requestOrigin)
        {
            if (requestOrigin == null)
            {
                return false;
            }

            string originSiteName = SiteInfoProvider.GetSiteNameFromUrl(requestOrigin);
            string currentSiteName = SiteContext.CurrentSite.SiteName;

            return String.Equals(originSiteName, currentSiteName, StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// If the Access-Control-Allow-Origin header is not present, it is added and set to given <paramref name="origin"/>. 
        /// In case the header is already set, it is replaced by given <paramref name="origin"/>.
        /// </summary>
        /// <param name="response">Response where the header will be set</param>
        /// <param name="origin">Origin URI as the value of the header</param>
        /// <remarks>
        /// Adding another origin when the header is already set will not work, browsers won't accept the response.
        /// Having allowed origin different than request origin is useless. Therefore is in that case the header value replaced by <paramref name="origin"/>
        /// </remarks>
        private static void SetAccessControlAllowOriginHeader(HttpResponseBase response, string origin)
        {
            if (response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME] == null)
            {
                // AppendHeader has some additional logic, so using this instead adding header directly using indexer
                response.AppendHeader(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME, origin);
            }
            else
            {
                // Header is already present so we can directly overwrite it
                response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME] = origin;
            }
        }
    }
}
