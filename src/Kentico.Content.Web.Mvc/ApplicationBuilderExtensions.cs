using Kentico.Web.Mvc;

namespace Kentico.Content.Web.Mvc
{
    /// <summary>
    /// Provides extension methods related to Kentico ASP.NET MVC integration features.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Transparently handles preview URLs and also disables output caching in preview mode.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static void UsePreview(this ApplicationBuilder builder)
        {
            var module = new PreviewFeatureModule();
            builder.RegisterModule(module);
        }
    }
}
