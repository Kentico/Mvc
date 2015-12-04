using System.Web.Mvc;

namespace Kentico.Web.Mvc
{
    internal class HttpErrorsController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }
    }
}