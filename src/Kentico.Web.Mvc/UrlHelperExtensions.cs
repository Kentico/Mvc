using System;
using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides methods to build URLs to Kentico content.
    /// </summary>
    public static class UrlHelperExtensions
    {
        private static ExtensionPoint<UrlHelper> mExtensionPoint;
        private static readonly object mLock = new Object();


        /// <summary>
        /// Returns an object that provides methods to build URLs to Kentico content.
        /// </summary>
        /// <param name="target">The instance of the <see cref="UrlHelper"/> class.</param>
        /// <returns>The object that provides methods to build URLs to Kentico content.</returns>
        public static ExtensionPoint<UrlHelper> Kentico(this UrlHelper target)
        {
            lock (mLock)
            {
                if (mExtensionPoint == null || mExtensionPoint.Target != target)
                {
                    mExtensionPoint = new ExtensionPoint<UrlHelper>(target);
                }

                return mExtensionPoint;
            }
        }
    }
}
