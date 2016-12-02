using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.SiteProvider;

using Kentico.Ecommerce;

using LearningKit.Models.Products;

namespace LearningKit.Controllers
{
    public class ProductController : Controller
    {
        private readonly string siteName = SiteContext.CurrentSiteName;
        private readonly IShoppingService shoppingService;
        private readonly IPricingService pricingService;
        private readonly IVariantRepository variantRepository;

        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the services and repositories.
        /// </summary>
        public ProductController()
        {
            //DocSection:InitializeServices
            shoppingService = new ShoppingService();
            pricingService = new PricingService();
            variantRepository = new KenticoVariantRepository();
            //EndDocSection:InitializeServices
        }

        //DocSection:DisplayProduct
        /// <summary>
        /// Displays a product detail page of a product specified by ID of the product's page.
        /// </summary>
        /// <param name="id">Node ID of the product's page.</param>
        /// <param name="productAlias">Node alias of the product's page.</param>
        public ActionResult BasicDetail(int id, string productAlias)
        {
            // Gets the product from Kentico
            SKUTreeNode product = GetProduct(id);
            
            // If the product is not found or if it is not allowed for sale, redirects to error 404
            if ((product == null) || (product.SKU == null) || !product.SKU.SKUEnabled)
            {
                return HttpNotFound();
            }
            
            // Redirects if the specified page alias does not match
            if (!string.IsNullOrEmpty(productAlias) && !product.NodeAlias.Equals(productAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Detail", new { id = product.NodeID, productAlias = product.NodeAlias });
            }
            
            // Initializes the view model of the product with a calculated price
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            ProductViewModel viewModel = new ProductViewModel(product, pricingService.CalculatePrice(product.SKU, cart));
            
            // Displays the product detail page
            return View(viewModel);
        }
        
        /// <summary>
        /// Retrieves the product specified by ID of the product's page.
        /// </summary>
        /// <param name="nodeID">Node ID of the product's page.</param>
        private SKUTreeNode GetProduct(int nodeID)
        {
            // Gets the page with the node ID
            TreeNode node = DocumentHelper.GetDocuments()
                            .LatestVersion(false)
                            .Published(true)
                            .OnSite(siteName)
                            .Culture("en-US")
                            .CombineWithDefaultCulture()
                            .WhereEquals("NodeID", nodeID)
                            .FirstOrDefault();
            
            // If the found page is not a product, returns null
            if (node == null || !node.IsProduct())
            {
                return null;
            }
            
            // Loads specific fields of the product's product page type from the database
            node.MakeComplete(true);
            
            // Returns the found page as a product page
            return node as SKUTreeNode;
        }
        //EndDocSection:DisplayProduct

        //DocSection:DisplayVariant
        /// <summary>
        /// Displays product detail page of a product or a product variant specified by ID of the product's or variant's page.
        /// </summary>
        /// <param name="id">Node ID of the product's (variant's) page.</param>
        /// <param name="productAlias">Node alias of the product's (variant's) page.</param>
        public ActionResult Detail(int id, string productAlias)
        {
            // Gets the product from Kentico
            SKUTreeNode product = GetProduct(id);
            
            // If the product is not found or if it is not allowed for sale, redirects to error 404
            if ((product == null) || !product.SKU.SKUEnabled)
            {
                return HttpNotFound();
            }
            
            // Redirects if the specified page alias does not match
            if (!string.IsNullOrEmpty(productAlias) && !product.NodeAlias.Equals(productAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Detail", new { id = product.NodeID, productAlias = product.NodeAlias });
            }
            
            // Gets all product variants of the product
            List<Variant> variants = variantRepository.GetByProductId(product.NodeSKUID).OrderBy(v => v.VariantPrice).ToList();
            
            // Selects the first product variant
            Variant selectedVariant = variants.FirstOrDefault();
            
            // Calculates the price of the product or the variant
            ShoppingCart cart = shoppingService.GetCurrentShoppingCart();
            ProductPrice priceDetail = selectedVariant != null ? pricingService.CalculatePrice(selectedVariant, cart) : pricingService.CalculatePrice(product.SKU, cart);
            
            // Initializes the view model of the product or product variant
            ProductViewModel viewModel = new ProductViewModel(product, priceDetail, variants, selectedVariant?.VariantSKUID ?? 0);
            
            // Displays the product detail page
            return View(viewModel);
        }
        
        /// <summary>
        /// Loads information about the demanded variant to change the page content.
        /// </summary>
        /// <param name="variantID">ID of the selected variant.</param>
        [HttpPost]
        public JsonResult Variant(int variantID)
        {
            // Gets SKU information based on the variant's ID
            SKUInfo variant = SKUInfoProvider.GetSKUInfo(variantID);
            
            // If the variant is null, returns null
            if (variant == null)
            {
                return null;
            }
            
            // Calculates the price of the variant
            ProductPrice variantPrice = pricingService.CalculatePrice(variant, shoppingService.GetCurrentShoppingCart());
            
            // Finds out whether the variant is in stock
            bool isInStock = variant.SKUTrackInventory == TrackInventoryTypeEnum.Disabled || variant.SKUAvailableItems > 0;
            
            // Creates a JSON response for the JavaScript that switches the variants
            var response = new
            {
                totalPrice = variantPrice.Currency.FormatPrice(variantPrice.Price),
                inStock = isInStock,
                stockMessage = isInStock ? "Yes" : "No"
            };
            
            // Returns the response
            return Json(response);
        }
        //EndDocSection:DisplayVariant
    }
}