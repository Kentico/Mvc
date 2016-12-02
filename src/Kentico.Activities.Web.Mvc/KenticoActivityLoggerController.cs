using System;
using System.Web.Mvc;

using CMS.WebAnalytics;

namespace Kentico.Activities.Web.Mvc
{
    /// <summary>
    /// Controller responsible for activity logging via javascript. Provides method for logging an activity
    /// and method which returns script which logs activity via javascript AJAX on client side.
    /// </summary>
    /// <remarks>
    /// Call <c>routes.Kentico().MapActivitiesRoutes();</c> before you register all your routes. Furthermore, include 
    /// script tag in your layout <c>@Scripts.Render(Url.RouteUrl("KenticoLogActivityScript"))</c> which is
    /// <c>@Scripts.Render("Kentico.Activities/Logger/Logger.js")</c> or create AJAX post request <c>Kentico.Activities/Logger/Log</c>.
    /// Make sure that the AJAX call is performed at every page all required fields are filled correctly (<see cref="KenticoActivityLoggerController.Log"/>).
    /// </remarks>
    public class KenticoActivityLoggerController : Controller
    {
        private readonly Lazy<PagesActivityLogger> mPagesActivityLogger = new Lazy<PagesActivityLogger>(true);


        private PagesActivityLogger PagesActivityLogger => mPagesActivityLogger.Value;


        /// <summary>
        /// Logs activities (pagevisit, landingpage).
        /// </summary>
        [HttpPost]
        public void Log(string title, string url, string referrer)
        {
            LogExternalSearchActivity(url, referrer);
            LogLandingPageActivity(title, url, referrer);
            LogPageVisitActivity(title, url, referrer);
        }


        /// <summary>
        /// Returns javascript file which calls <see cref="Log"/> action via AJAX immediately after it is loaded.
        /// </summary>
        public ActionResult Script()
        {
            var logUrl = Url.Action("Log");
            return Content(Scripts.Logger + $"('{logUrl}');", "application/javascript");
        }


        private void LogExternalSearchActivity(string url, string referrer)
        {
            Uri refererUri;
            if (Uri.TryCreate(referrer, UriKind.Absolute, out refererUri))
            {
                PagesActivityLogger.LogExternalSearch(refererUri, null, url, referrer);
            }
        }


        private void LogLandingPageActivity(string title, string url, string referrer)
        {
            PagesActivityLogger.LogLandingPage(title, null, url, referrer);
        }


        private void LogPageVisitActivity(string title, string url, string referrer)
        {
            PagesActivityLogger.LogPageVisit(title, null, null, url, referrer);
        }
    }
}