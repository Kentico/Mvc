using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Creates an object that implements the <see cref="IHttpHandler"/> interface and passes the request context to it.
    /// Configures the current thread to use the culture specified by the 'culture' URL parameter.
    /// </summary>
    public class MultiCultureMvcRouteHandler : MvcRouteHandler
    {
        private readonly CultureInfo defaultCulture;


        /// <summary>
        /// Creates a new instance of the <see cref="MultiCultureMvcRouteHandler"/> class.
        /// </summary>
        /// <param name="defaultCulture">Culture used when the requested culture does not exist.</param>
        public MultiCultureMvcRouteHandler(CultureInfo defaultCulture)
        {
            this.defaultCulture = defaultCulture;
        }


        /// <summary>
        /// Returns the HTTP handler by using the specified HTTP context. 
        /// <see cref="Thread.CurrentCulture"/> and <see cref="Thread.CurrentUICulture"/> of the current thread are set to the culture specified by the 'culture' URL parameter.
        /// </summary>
        /// <param name="requestContext">Request context.</param>
        /// <returns>HTTP handler.</returns>
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            // Get the requested culture from the route
            var cultureName = requestContext.RouteData.Values["culture"].ToString();

            CultureInfo culture;
            try
            {
                culture = new CultureInfo(cultureName);
            }
            catch
            {
                culture = defaultCulture;
            }

            // Set the culture
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            return base.GetHttpHandler(requestContext);
        }
    }
}