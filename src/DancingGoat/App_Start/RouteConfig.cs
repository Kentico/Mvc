using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

using Kentico.Web.Mvc;
using DancingGoat.Infrastructure;

namespace DancingGoat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var defaultCulture = CultureInfo.GetCultureInfo("en-US");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Map routes to Kentico HTTP handlers first as some Kentico URLs might be matched by the default ASP.NET MVC route resulting in displaying pages without images
            routes.Kentico().MapRoutes();

            var route = routes.MapRoute(
                name: "Article",
                url: "{culture}/Articles/{id}/{pageAlias}",
                defaults: new { culture = defaultCulture.Name, controller = "Articles", action = "Show" },
                constraints: new { culture = new SiteCultureConstraint("DancingGoatMvc"), id = new IntRouteConstraint() }
            );

            // A route value determines the culture of the current thread
            route.RouteHandler = new MultiCultureMvcRouteHandler(defaultCulture);

            route = routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}",
                defaults: new { culture = defaultCulture.Name, controller = "Home", action = "Index" },
                constraints: new { culture = new SiteCultureConstraint("DancingGoatMvc") }
            );

            // A route value determines the culture of the current thread
            route.RouteHandler = new MultiCultureMvcRouteHandler(defaultCulture);

            // Display a custom view when no route is found
            routes.MapRoute(
                name: "NotFound",
                url: "{*url}",
                defaults: new { controller = "HttpErrors", action = "NotFound" }
            );
        }
    }
}
