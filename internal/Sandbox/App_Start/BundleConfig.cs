using System.Web.Optimization;

namespace Sendbox
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            RegisterJqueryBundle(bundles);
            RegisterJqueryValidationBundle(bundles);
        }


        private static void RegisterJqueryBundle(BundleCollection bundles)
        {
            var bundle = new ScriptBundle("~/bundles/jquery")
            {
                CdnPath = "//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.1.4.min.js",
                CdnFallbackExpression = "window.jQuery"
            };

            bundle.Include("~/Scripts/jquery-{version}.js");

            bundles.Add(bundle);
        }

        
        private static void RegisterJqueryValidationBundle(BundleCollection bundles)
        {
            var bundle = new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/jquery.unobtrusive-ajax.min.js");

            bundles.Add(bundle);
        }
    }
}