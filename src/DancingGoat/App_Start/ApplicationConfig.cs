using Kentico.Web.Mvc;
using Kentico.CampaignLogging.Web.Mvc;
using Kentico.Content.Web.Mvc;

namespace DancingGoat
{
    public class ApplicationConfig
    {
        public static void RegisterFeatures(ApplicationBuilder builder)
        {
            builder.UsePreview();
            builder.UseDataAnnotationsLocalization();
            builder.UseNotFoundHandler();
            builder.UseResourceSharingWithAdministration();
            builder.UseCampaignLogger();
        }
    }
}