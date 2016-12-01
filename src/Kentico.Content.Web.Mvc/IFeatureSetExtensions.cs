using Kentico.Web.Mvc;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Provides extension methods for the <see cref="IFeatureSet"/> interface.
    /// </summary>
    public static class IFeatureSetExtensions
    {
        /// <summary>
        /// Returns a feature that provides information about preview.
        /// </summary>
        /// <param name="features">The set of features.</param>
        /// <returns>A feature that provides information about preview.</returns>
        public static IPreviewFeature Preview(this IFeatureSet features)
        {
            return features.GetFeature<IPreviewFeature>();
        }
    }
}
