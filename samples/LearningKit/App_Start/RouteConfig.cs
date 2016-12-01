using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;


using Kentico.Web.Mvc;
using Kentico.Newsletters.Web.Mvc;
using Kentico.Activities.Web.Mvc;

namespace LearningKit
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Maps routes for general Kentico HTTP handlers.
            // Must be first, since some Kentico URLs may be matched by the default ASP.NET MVC routes,
            // which can result in pages being displayed without images.
            routes.Kentico().MapRoutes();

            //DocSection:MarketingEmailTrackingRoutes
            // Maps routes used for tracking of marketing emails
            // Registers the "CMSModules/Newsletters/CMSPages/Track.ashx" route for opened marketing email tracking
            // Registers the "CMSModules/Newsletters/CMSPages/Redirect.ashx" route for clicked link tracking
            routes.Kentico().MapOpenedEmailHandlerRoute();
            routes.Kentico().MapEmailLinkHandlerRoute();
            //EndDocSection:MarketingEmailTrackingRoutes

            //DocSection:VisitorActionRoute
            // Maps routes for logging of on-line marketing campaign page visits and page-related activities
            routes.Kentico().MapActivitiesRoutes();
            //EndDocSection:VisitorActionRoute

            //DocSection:ListingRoute
            routes.MapRoute(
                name: "Store",
                url: "Store/{controller}",
                defaults: new { action = "Index" },
                constraints: new { controller = "LearningProductType" }
            );
            //EndDocSection:ListingRoute

            //DocSection:ProductRoute
            routes.MapRoute(
                name: "Product",
                url: "Product/{id}/{productAlias}",
                defaults: new { controller = "Product", action = "Detail" },
                constraints: new { id = new IntRouteConstraint() }
            );
            //EndDocSection:ProductRoute

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
