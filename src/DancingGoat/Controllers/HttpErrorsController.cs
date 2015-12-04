using System.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class HttpErrorsController : Controller
    {
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }
    }
}