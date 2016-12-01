using CMS.Ecommerce;

using Kentico.Ecommerce;

namespace LearningKit.Models.Products
{
    //DocSection:ProductListingModel
    public class ProductListItemViewModel
    {
        public readonly ProductPrice PriceDetail;
        public readonly string Name;
        public readonly string ImagePath;
        public readonly string PublicStatusName;
        public readonly bool Available;
        public readonly int ProductPageID;
        public readonly string ProductPageAlias;
        
        /// <summary>
        /// Creates a model from an item from a product listing.
        /// </summary>
        /// <param name="productPage">Product's page.</param>
        /// <param name="priceDetail">Price of the product.</param>
        /// <param name="publicStatusName">Display name of the product's public status.</param>
        public ProductListItemViewModel(SKUTreeNode productPage, ProductPrice priceDetail, string publicStatusName)
        {
            // Sets the page information
            Name = productPage.DocumentName;
            ProductPageID = productPage.NodeID;
            ProductPageAlias = productPage.NodeAlias;
            
            // Sets the SKU information
            ImagePath = productPage.SKU.SKUImagePath;
            Available = !productPage.SKU.SKUSellOnlyAvailable || productPage.SKU.SKUAvailableItems > 0;
            PublicStatusName = publicStatusName;
            
            // Sets the price
            PriceDetail = priceDetail;
        }
    }
    //EndDocSection:ProductListingModel
}