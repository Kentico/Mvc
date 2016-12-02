using Kentico.Web.Mvc;
using Kentico.CampaignLogging.Web.Mvc;
using Kentico.Content.Web.Mvc;

namespace LearningKit
{
    public class ApplicationConfig
    {
        public static void RegisterFeatures(ApplicationBuilder builder)
        {
            builder.UsePreview();
            builder.UseDataAnnotationsLocalization();
            builder.UseNotFoundHandler();
            //DocSection:CampaignLogger
            builder.UseCampaignLogger();
            //EndDocSection:CampaignLogger
        }
    }
}