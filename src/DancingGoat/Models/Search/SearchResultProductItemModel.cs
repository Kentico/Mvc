using System.Web;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Helpers;

using Kentico.Ecommerce;
using Kentico.Search;

namespace DancingGoat.Models.Search
{
    public class SearchResultProductItemModel : SearchResultPageItemModel
    {
        public string Description { get; set; }


        public string ShortDescription { get; set; }


        public ProductPrice PriceDetail { get; set; }


        public SearchResultProductItemModel(SearchFields fields, SKUTreeNode skuTreeNode, ProductPrice priceDetail)
            : base(fields, skuTreeNode)
        {
            Description = skuTreeNode.DocumentSKUDescription;
            ShortDescription = HTMLHelper.StripTags(skuTreeNode.DocumentSKUShortDescription, false);
            PriceDetail = priceDetail;

            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            Url = urlHelper.Action("Detail", "Product",
                new {id = skuTreeNode.NodeID, productAlias = skuTreeNode.NodeAlias});
        }
    }
}