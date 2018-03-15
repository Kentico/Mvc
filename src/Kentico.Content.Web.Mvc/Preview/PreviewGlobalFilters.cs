using System.Linq;
using System.Web.Mvc;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Class that provides registration of all the required global filters for the page preview feature.
    /// </summary>
    internal static class PreviewGlobalFilters
    {
        /// <summary>
        /// Registers global filters for the Preview feature.
        /// </summary>
        public static void Register()
        {
            var globalFilters = GlobalFilters.Filters;

            // Add a filter to disable output cache for preview
            if (!globalFilters.Any(f => f.Instance.GetType().Equals(typeof(PreviewOutputCacheFilter))))
            {
                globalFilters.Add(new PreviewOutputCacheFilter());
            }
        }
    }
}
