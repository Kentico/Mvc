using System;
using System.Web;

using CMS.DataEngine;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides Kentico ASP.NET MVC integration.
    /// </summary>
    internal sealed class ApplicationHttpModule : IHttpModule
    {
        /// <summary>
        /// Releases the resources used by a module.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose
        }


        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="application">The ASP.NET application that defines common methods, properties, and events.</param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += HandleBeginRequest;

            // Build the request pipeline from registered modules that provide Kentico ASP.NET MVC integration features.
            // The modules are registered in the Application_Start method that is executed before initialization of HTTP modules.
            ApplicationBuilder.Current.Build(application);
        }


        private void HandleBeginRequest(object sender, EventArgs e)
        {
            CMSApplication.Init();
        }
    }
}
