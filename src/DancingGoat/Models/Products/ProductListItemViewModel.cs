using CMS.Ecommerce;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Products
{
    public class ProductListItemViewModel
    {
        public ProductPrice PriceDetail { get; }


        public string Name { get; }


        public string ImagePath { get; }


        public string PublicStatusName { get; }


        public bool Available { get; }


        public int ProductPageID { get; }


        public string ProductPageAlias { get; }


        public ProductListItemViewModel(SKUTreeNode productPage, ProductPrice priceDetail, string publicStatusName)
        {
            // Set page information
            Name = productPage.DocumentName;
            ProductPageID = productPage.NodeID;
            ProductPageAlias = productPage.NodeAlias;

            // Set SKU information
            ImagePath = productPage.SKU.SKUImagePath;
            Available = !productPage.SKU.SKUSellOnlyAvailable || productPage.SKU.SKUAvailableItems > 0;
            PublicStatusName = publicStatusName;

            // Set additional info
            PriceDetail = priceDetail;
        }
    }
}