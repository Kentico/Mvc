using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Kentico.Web.Mvc;

namespace Sandbox
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Enable and configure selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);
        }
    }
}
