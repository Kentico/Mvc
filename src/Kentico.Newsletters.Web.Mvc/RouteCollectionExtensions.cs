using System;
using System.Web.Routing;

using CMS.Newsletters;
using CMS.SiteProvider;
using Kentico.Web.Mvc;

namespace Kentico.Newsletters.Web.Mvc
{
    /// <summary>
    /// Extends a <see cref="RouteCollection"/> object for MVC routing.
    /// </summary>
    public static class RouteCollectionExtensions
    {
        /// <summary>
        /// Maps handler responsible for tracking opened emails to <paramref name="routeUrl"/>.
        /// </summary>
        /// <param name="extensionPoint">The object that provides extensibility for <see cref="RouteCollection"/>.</param>
        /// <param name="routeUrl">URL where handler responsible for tracking opened emails is mapped. See the remarks section when using non-default URL.</param>
        /// <remarks>
        /// The Kentico administration application uses <see cref="EmailTrackingLinkHelper"/> to obtain tracking URLs when sending emails. When non-default URL for <paramref name="routeUrl"/> is used,
        /// the <see cref="EmailTrackingLinkHelper.GetOpenedEmailTrackingPageInternal(SiteInfo)"/> method has to be customized accordingly.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="extensionPoint"/> is null.</exception>
        /// <returns>Route where handler responsible for tracking opened emails is mapped.</returns>
        public static Route MapOpenedEmailHandlerRoute(this ExtensionPoint<RouteCollection> extensionPoint, string routeUrl = EmailTrackingLinkHelper.DEFAULT_OPENED_EMAIL_TRACKING_ROUTE_HANDLER_URL)
        {
            if (extensionPoint == null)
            {
                throw new ArgumentNullException(nameof(extensionPoint));
            }

            var routes = extensionPoint.Target;
            
            var constraints = new RouteValueDictionary(new { controller = String.Empty, action = String.Empty });
            var defaults = new RouteValueDictionary();

            Route route = new Route(routeUrl, defaults, constraints, new RouteHandlerWrapper<OpenEmailTracker>());

            using (routes.GetWriteLock())
            {
                routes.Add(route);
            }

            return route;
        }


        /// <summary>
        /// Maps handler responsible for tracking clicked links in emails to <paramref name="routeUrl"/>.
        /// </summary>
        /// <param name="extensionPoint">The object that provides extensibility for <see cref="RouteCollection"/>.</param>
        /// <param name="routeUrl">URL where handler responsible for tracking clicked links in emails is mapped. See the remarks section when using non-default URL.</param>
        /// <remarks>
        /// The Kentico administration application uses <see cref="EmailTrackingLinkHelper"/> to obtain tracking URLs when sending emails. When non-default URL for <paramref name="routeUrl"/> is used,
        /// the <see cref="EmailTrackingLinkHelper.GetClickedLinkTrackingPageInternal(SiteInfo)"/> method has to be customized accordingly.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="extensionPoint"/> is null.</exception>
        /// <returns>Route where handler responsible for tracking clicked links in emails is mapped.</returns>
        public static Route MapEmailLinkHandlerRoute(this ExtensionPoint<RouteCollection> extensionPoint, string routeUrl = EmailTrackingLinkHelper.DEFAULT_LINKS_TRACKING_ROUTE_HANDLER_URL)
        {
            if (extensionPoint == null)
            {
                throw new ArgumentNullException(nameof(extensionPoint));
            }

            var routes = extensionPoint.Target;
            
            var constraints = new RouteValueDictionary(new { controller = String.Empty, action = String.Empty });
            var defaults = new RouteValueDictionary();

            Route route = new Route(routeUrl, defaults, constraints, new RouteHandlerWrapper<LinkTracker>());

            using (routes.GetWriteLock())
            {
                routes.Add(route);
            }

            return route;
        }
    }
}
