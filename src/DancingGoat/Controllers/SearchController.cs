using System;
using System.Collections.Generic;
using System.Web.Mvc;

using PagedList;

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
            int totalItemsCount;
            var pageNumber = (page ?? 1);
            pageNumber = Math.Max(pageNumber, 1);

            var searchResults = mService.Search(searchText, pageNumber - 1, PAGE_SIZE, out totalItemsCount);

            var model = new SearchResultsModel
            {
                Items = new StaticPagedList<SearchResultItem>(searchResults ?? new List<SearchResultItem>(), pageNumber, PAGE_SIZE, totalItemsCount),
                Query = searchText
            };

            return View(model);
        }
    }
}