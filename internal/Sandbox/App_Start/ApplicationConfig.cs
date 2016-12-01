using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace Sandbox
{
    public class ApplicationConfig
    {
        public static void RegisterFeatures(ApplicationBuilder builder)
        {
            builder.UsePreview();
            builder.UseDataAnnotationsLocalization();
            builder.UseNotFoundHandler();
        }
    }
}