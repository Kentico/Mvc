using System;
using System.Collections.Generic;
using System.Web;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Configures the request pipeline using Kentico ASP.NET MVC integration features.
    /// </summary>
    public sealed class ApplicationBuilder
    {
        /// <summary>
        /// The current instance of the <see cref="ApplicationBuilder"/> class.
        /// </summary>
        private static readonly ApplicationBuilder mInstance = new ApplicationBuilder();
        

        /// <summary>
        /// A collection of request pipeline modules that provide Kentico ASP.NET MVC integration features.
        /// </summary>
        private readonly List<IModule> mModules;


        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBuilder"/> class.
        /// </summary>
        private ApplicationBuilder()
        {
            mModules = new List<IModule>();
        }

        
        /// <summary>
        /// Gets the current application builder.
        /// </summary>
        public static ApplicationBuilder Current
        {
            get
            {
                return mInstance;
            }
        }


        /// <summary>
        /// Registers a request pipeline module that provides Kentico ASP.NET MVC integration features.
        /// </summary>
        /// <param name="module">The module to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="module"/> is null.</exception>
        internal void RegisterModule(IModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            mModules.Add(module);
        }


        /// <summary>
        /// Builds the request pipeline from registered modules that provide Kentico ASP.NET MVC integration features.
        /// </summary>
        /// <param name="application">The ASP.NET application that defines common methods, properties, and events.</param>
        /// <exception cref="ArgumentNullException"><paramref name="application"/> is null.</exception>
        internal void Build(HttpApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            foreach (var module in mModules)
            {
                module.Initialize(application);
            }
        }
    }
}
