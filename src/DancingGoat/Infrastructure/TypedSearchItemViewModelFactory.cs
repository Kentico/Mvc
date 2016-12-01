using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;

using Kentico.Search;

using DancingGoat.Models.Search;
using DancingGoat.Services;


namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Provides methods for conversion from <see cref="SearchResultItem{TInfo}"/> to particular <see cref="SearchResultItemModel"/>.
    /// </summary>
    public class TypedSearchItemViewModelFactory
    {
        private readonly ICalculationService mCalculationService;


        public TypedSearchItemViewModelFactory(ICalculationService calculationService)
        {
            mCalculationService = calculationService;
        }


        /// <summary>
        /// Creates a search view model according to the runtime type of <paramref name="searchResultItem"/>.
        /// </summary>
        public SearchResultItemModel GetTypedSearchResultItemModel(SearchResultItem<BaseInfo> searchResultItem)
        {
            var fields = searchResultItem.Fields;
            var data = (dynamic) searchResultItem.Data;
            return GetViewModelForSearchItem(fields, data);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchFields fields, TreeNode treeNode)
        {
            return new SearchResultPageItemModel(fields, treeNode);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchFields fields, SKUTreeNode skuTreeNode)
        {
            var price = mCalculationService.CalculateListingPrice(skuTreeNode.SKU);
            return new SearchResultProductItemModel(fields, skuTreeNode, price);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchFields fields, Article article)
        {
            return new SearchResultArticleItemModel(fields, article);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchFields fields, AboutUs aboutUs)
        {
            return new SearchResultAboutUsItemModel(fields, aboutUs);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchFields fields, AboutUsSection aboutUsSection)
        {
            return new SearchResultAboutUsSectionItemModel(fields, aboutUsSection);
        }


        private SearchResultItemModel GetViewModelForSearchItem(SearchFields fields, Cafe cafe)
        {
            return new SearchResultCafeItemModel(fields, cafe);
        }
    }
}