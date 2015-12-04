using System;
using System.Web.Routing;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides methods to add routes to Kentico HTTP handlers.
    /// </summary>
    public static class RouteCollectionExtensions
    {
        private static ExtensionPoint<RouteCollection> mExtensionPoint;
        private static readonly object mLock = new Object();


        /// <summary>
        /// Returns an object that provides methods to add routes to Kentico HTTP handlers.
        /// </summary>
        /// <param name="target">The instance of the <see cref="RouteCollection"/> class.</param>
        /// <returns>The object that provides methods to add routes to Kentico HTTP handlers.</returns>
        public static ExtensionPoint<RouteCollection> Kentico(this RouteCollection target)
        {
            lock (mLock)
            {
                if (mExtensionPoint == null || mExtensionPoint.Target != target)
                {
                    mExtensionPoint = new ExtensionPoint<RouteCollection>(target);
                }

                return mExtensionPoint;
            }
        }
    }
}
