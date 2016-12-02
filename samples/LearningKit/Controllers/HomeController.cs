using System;
using System.Web.Mvc;

namespace LearningKit.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }
    }
}