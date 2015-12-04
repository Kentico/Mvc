using System.Collections;
using System.Web;

namespace Kentico.Web.Mvc
{
    /// <summary>
    /// Provides extension methods for the <see cref="HttpContext"/> and <see cref="HttpContextBase"/> classes.
    /// </summary>
    public static class HttpContextExtensions
    {
        private const string FEATURE_BUNDLE_KEY = "Kentico.Features";
        private static readonly object mLock = new object();


        /// <summary>
        /// Returns a feature set for the specified request.
        /// </summary>
        /// <param name="context">The object that encapsulates information about an HTTP request.</param>
        /// <returns>A feature set for the specified request.</returns>
        public static IFeatureSet Kentico(this HttpContext context)
        {
            return GetOrCreateFeaturesFromItems(context.Items);
        }


        /// <summary>
        /// Returns a feature set for the specified request.
        /// </summary>
        /// <param name="context">The object that encapsulates information about an HTTP request.</param>
        /// <returns>A feature set for the specified request.</returns>
        public static IFeatureSet Kentico(this HttpContextBase context)
        {
            return GetOrCreateFeaturesFromItems(context.Items);
        }


        private static IFeatureSet GetOrCreateFeaturesFromItems(IDictionary items)
        {
            IFeatureSet features;
            lock (mLock)
            {
                if (items.Contains(FEATURE_BUNDLE_KEY))
                {
                    features = (IFeatureSet)items[FEATURE_BUNDLE_KEY];
                }
                else
                {
                    features = new FeatureSet();
                    items.Add(FEATURE_BUNDLE_KEY, features);
                }
            }

            return features;
        }
    }
}
