using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

using CMS.Ecommerce;

using Kentico.Ecommerce;

namespace LearningKit.Models.Products
{
    public class ProductViewModel
    {
        //DocSection:BasicModel
        public readonly ProductPrice PriceDetail;
        public readonly string Name;
        public readonly string Description;
        public readonly string ShortDescription;
        public readonly int SKUID;
        public readonly string ImagePath;
        public readonly int ProductPageID;
        public readonly string ProductPageAlias;
        public readonly bool IsInStock;
        
        /// <summary>
        /// Creates a new product model.
        /// </summary>
        /// <param name="productPage">Product's page.</param>
        /// <param name="priceDetail">Price of the product.</param>
        public ProductViewModel(SKUTreeNode productPage, ProductPrice priceDetail)
        {
            // Fills the page information   
            Name = productPage.DocumentName;
            Description = productPage.DocumentSKUDescription;
            ShortDescription = productPage.DocumentSKUShortDescription;
            ProductPageID = productPage.NodeID;
            ProductPageAlias = productPage.NodeAlias;
            
            // Fills the SKU information
            SKUInfo sku = productPage.SKU;
            SKUID = sku.SKUID;
            ImagePath = sku.SKUImagePath;
            IsInStock = sku.SKUTrackInventory == TrackInventoryTypeEnum.Disabled ||
                        sku.SKUAvailableItems > 0;
            
            PriceDetail = priceDetail;
        }
        //EndDocSection:BasicModel


        //DocSection:VariantModel
        public readonly List<Variant> ProductVariants;
        public SelectList VariantSelectList { get; set; }
        public int SelectedVariantID { get; set; }
        
        /// <summary>
        /// Creates a new product model with variants.
        /// </summary>
        /// <param name="productPage">Product's page.</param>
        /// <param name="priceDetail">Price of the selected variant.</param>
        /// <param name="variants">Collection of selectable variants.</param>
        /// <param name="selectedVariantID">ID of the selected variant.</param>
        public ProductViewModel(SKUTreeNode productPage, ProductPrice priceDetail, List<Variant> variants, int selectedVariantID)
            : this(productPage, priceDetail)
        {
            // Fills the selectable variants
            ProductVariants = variants;
            
            // Continues if the product has any variants
            if (variants.Any())
            {
                // Pre select variant
                var selectedVariant = variants.FirstOrDefault(v => v.VariantSKUID == selectedVariantID);
                
                if (selectedVariant != null)
                {
                    IsInStock = !selectedVariant.InventoryTracked || selectedVariant.AvailableItems > 0;
                    SelectedVariantID = selectedVariantID;
                }
                
                // Creates a list of product variants
                VariantSelectList = new SelectList(variants.Select(v => new SelectListItem
                {
                    Text = string.Join(", ", v.ProductAttributes.Select(a => a.SKUName)),
                    Value = v.VariantSKUID.ToString()
                }), "Value", "Text");
            }
        }
        //EndDocSection:VariantModel
    }
}