using System.Web.Mvc;

using DancingGoat.Models.Search;
using Kentico.Search;

namespace DancingGoat.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService mService;
        private const int PAGE_SIZE = 5;


        public SearchController(SearchService searchService)
        {
            mService = searchService;
        }


        // GET: Search
        [ValidateInput(false)]
        public ActionResult Index(string searchText, int? page)
        {
            var pageIndex = (page ?? 1) - 1;
            int totalItemsCount;
            var model = new SearchResults()
            {
                Items = mService.Search(searchText, pageIndex, PAGE_SIZE, out totalItemsCount),
                PageIndex = pageIndex,
                PageSize = PAGE_SIZE,
                Query = searchText,
                TotalItemCount = totalItemsCount
            };

            return View(model);
        }
    }
}