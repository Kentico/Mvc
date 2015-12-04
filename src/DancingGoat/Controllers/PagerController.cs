using System.Web.Mvc;

using DancingGoat.Models.Pager;

namespace DancingGoat.Controllers
{
    public class PagerController : Controller
    {
        // GET: Pager
        [ValidateInput(false)]
        public ActionResult Index(PagerModel model)
        {
            return PartialView("_Pager", model);
        }
    }
}