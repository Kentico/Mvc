//DocSection:Using
using System;
using System.Collections.Generic;
using System.Web.Mvc;

using CMS.DataEngine;

using Kentico.Search;
//EndDocSection:Using

using LearningKit.Models.Search;

namespace LearningKit.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;

        //DocSection:SearchController
        // Adds the smart search indexes that will be used when performing a search and sets item count per page
        public static readonly string[] searchIndexes = new string[] { "MVCSite.Index" };
        private const int PAGE_SIZE = 10;
        
        /// <summary>
        /// Constructor.
        /// Initializes the search service that takes care of performing searches. You can use a dependency injection container to initialize the service.
        /// </summary>
        public SearchController()
        {
            searchService = new SearchService();
        }
        
        /// <summary>
        /// Performs a search and displays its result.
        /// </summary>
        /// <param name="searchText">Query for search.</param>
        [ValidateInput(false)]
        public ActionResult SearchIndex(string searchText)
        {
            // Displays the search page without any search results if the query is empty
            if (String.IsNullOrWhiteSpace(searchText))
            {
                // Creates a model representing empty search results
                SearchResultModel emptyModel = new SearchResultModel
                {
                    Items = new List<SearchResultItemModel>(),
                    Query = String.Empty
                };
                
                return View(emptyModel);
            }
            
            // Searches with the smart search through Kentico and gets the search result
            SearchResult searchResult = searchService.Search(new SearchOptions(searchText, searchIndexes)
            {
                CultureName = "en-us",
                CombineWithDefaultCulture = true,
                PageSize = PAGE_SIZE
            });
            
            // Creates a list for search result item models
            List<SearchResultItemModel> itemModels = new List<SearchResultItemModel>();
            
            // Loops through the search result items
            foreach (SearchResultItem<BaseInfo> searchResultItem in searchResult.Items)
            {
                // Adds data to a view model. You can adjust the logic here to get to the object specific fields
                itemModels.Add(new SearchResultItemModel(searchResultItem.Fields));
            }
            
            // Creates a model with the search result items
            SearchResultModel model = new SearchResultModel
            {
                Items = itemModels,
                Query = searchText
            };
            
            return View(model);
        }
        //EndDocSection:SearchController
    }
}