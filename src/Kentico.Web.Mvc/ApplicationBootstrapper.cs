using System.Web;

using CMS.DataEngine;
using CMS.Base;

[assembly: PreApplicationStartMethod(typeof(Kentico.Web.Mvc.ApplicationBootstrapper), "Run")]

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Initializes Kentico integration with ASP.NET MVC. This class is for internal use only.
    /// </summary>
    public static class ApplicationBootstrapper
    {
        /// <summary>
        /// Runs the bootstrapper process.
        /// </summary>
        public static void Run()
        {
            // Discover routes to Kentico HTTP handlers before the MVC application starts to allow adding them to the route table.
            // It is not possible to completely initialize Kentico yet as it requires access to the current HTTP request that is not available in this phase of application life-cycle.
            SystemContext.IsWebSite = true;
            CMSApplication.PreInit();

            // Register the module that provider Kentico ASP.NET MVC integration.
            HttpApplication.RegisterModule(typeof(ApplicationHttpModule));
        }
    }
}
