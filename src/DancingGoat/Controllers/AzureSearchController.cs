using Microsoft.Azure.Search.Models;
using MVCDemo_TestDemoApp.Helpers;
using System.Web.Mvc;

namespace DancingGoat.Controllers
{

    public class AzureSearchController : Controller
    {
        // GET: /AzureSearch/
        private AzureSearchHelper _AzureSearch = new AzureSearchHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string q = "", bool h = false, bool f = false, bool s = false, string fi = "", string sp = "")
        {
            //Create an object array to return
            object[] searchresults = new object[2];
            //Get the JsonResult
            JsonResult resultsdata = new JsonResult();
            resultsdata.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            resultsdata.Data = _AzureSearch.Search(q + "*", h, f, s, fi, sp);
            //Get the Facets section. This value is not accessible via the JsonResult object so we need to create a DocumentSearchResponse to retrieve it.
            DocumentSearchResponse dsrfacets = (DocumentSearchResponse)resultsdata.Data;

            searchresults[0] = resultsdata.Data;
            searchresults[1] = dsrfacets.Facets;

            //Return the array
            return Json(searchresults);
        }


        public ActionResult CreateIndex()
        {
            ViewData["message"] = _AzureSearch.CreateIndex();
            return Content(ViewData["message"].ToString());
        }

        public ActionResult LoadIndex()
        {
            ViewData["message"] = _AzureSearch.LoadIndex();
            return Content(ViewData["message"].ToString());
        }

        public ActionResult DeleteIndex()
        {
            ViewData["message"] = _AzureSearch.DeleteIndex();
            return Content(ViewData["message"].ToString());
        }
        
        public ActionResult Suggest(string search, bool fuzzy)
        {
            //Get the JsonResult
            JsonResult suggestiondata = new JsonResult();
            suggestiondata.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            suggestiondata.Data = _AzureSearch.Suggest(search, fuzzy);
            
            //Return the array
            return Json(suggestiondata);
        }
    }
}