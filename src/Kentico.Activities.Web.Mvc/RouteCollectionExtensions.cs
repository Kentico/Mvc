using System;
using System.Web.Mvc;
using System.Web.Routing;

using Kentico.Web.Mvc;

namespace Kentico.Activities.Web.Mvc
{
    /// <summary>
    /// Extends a <see cref="RouteCollection"/> object for MVC routing.
    /// </summary>
    public static class RouteCollectionExtensions
    {

        /// <summary>
        /// Adds Activities routes.
        /// </summary>
        /// <param name="extensionPoint">The object that provides methods to add Kentico Activities routes.</param>
        /// <remarks>Routes are fixed with namespace prefix.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="extensionPoint"/> is null.</exception>
        public static void MapActivitiesRoutes(this ExtensionPoint<RouteCollection> extensionPoint)
        {
            if (extensionPoint == null)
            {
                throw new ArgumentNullException(nameof(extensionPoint));
            }

            var routes = extensionPoint.Target;

            routes.MapRoute(
                name: "KenticoLogActivity",
                url: "Kentico.Activities/KenticoActivityLogger/Log",
                defaults: new { controller = "KenticoActivityLogger", action = "Log" },
                namespaces: new []{ typeof(RouteCollectionExtensions).Namespace }
                );

            routes.MapRoute(
                name: "KenticoLogActivityScript",
                url: "Kentico.Activities/KenticoActivityLogger/Logger.js",
                defaults: new { controller = "KenticoActivityLogger", action = "Script" },
                namespaces: new[] { typeof(RouteCollectionExtensions).Namespace }
                );
        }
    }
}
