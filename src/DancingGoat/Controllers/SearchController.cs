using System;
using System.Linq;
using System.Web.Mvc;

using Kentico.Search;

using DancingGoat.Models.Search;
using DancingGoat.Infrastructure;

using PagedList;


namespace DancingGoat.Controllers
{
    public class SearchController : Controller
    {
        private const string INDEX_NAME = "DancingGoatMvc.Index";
        private const int PAGE_SIZE = 5;
        private const int DEFAULT_PAGE_NUMBER = 1;

        private readonly ISearchService mSearchService;
        private readonly TypedSearchItemViewModelFactory mSearchItemViewModelFactory;


        public SearchController(ISearchService searchService, TypedSearchItemViewModelFactory searchItemViewModelFactory)
        {
            mSearchService = searchService;
            mSearchItemViewModelFactory = searchItemViewModelFactory;
        }


        // GET: Search
        [ValidateInput(false)]
        public ActionResult Index(string searchText, int page = DEFAULT_PAGE_NUMBER)
        {
            if (String.IsNullOrWhiteSpace(searchText))
            {
                var empty = new SearchResultsModel
                {
                    Items = new StaticPagedList<SearchResultItemModel>(Enumerable.Empty<SearchResultItemModel>(), page, PAGE_SIZE, 0)
                };
                return View(empty);
            }

            // Validate page number (starting from 1)
            page = Math.Max(page, DEFAULT_PAGE_NUMBER);

            var searchResults = mSearchService.Search(new SearchOptions(searchText, new [] { INDEX_NAME } )
            {
                PageNumber = page,
                PageSize = PAGE_SIZE
            });

            var searchResultItemModels = searchResults.Items
                .Select(searchResultItem => mSearchItemViewModelFactory.GetTypedSearchResultItemModel(searchResultItem));

            var model = new SearchResultsModel
            {
                Items = new StaticPagedList<SearchResultItemModel>(searchResultItemModels, page, PAGE_SIZE, searchResults.TotalNumberOfResults),
                Query = searchText
            };

            return View(model);
        }
    }
}