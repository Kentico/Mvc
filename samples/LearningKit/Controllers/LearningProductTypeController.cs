using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.SiteProvider;
using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Ecommerce;

using LearningKit.Models.Products;

namespace LearningKit.Controllers
{
    //DocSection:ListingController
    public class LearningProductTypeController : Controller
    {
        private readonly string siteName = SiteContext.CurrentSiteName;
        private readonly IShoppingService shoppingService;
        private readonly IPricingService pricingService;
        
        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the services.
        /// </summary>
        public LearningProductTypeController()
        {
            shoppingService = new ShoppingService();
            pricingService = new PricingService();
        }
        
        /// <summary>
        /// Displays a product listing page of the class's product page type.
        /// </summary>
        public ActionResult Index()
        {
            // Gets products of the product page type (via the generated page type code)
            List<LearningProductType> products = LearningProductTypeProvider.GetLearningProductTypes()
                .LatestVersion(false)
                .Published(true)
                .OnSite(siteName)
                .Culture("en-US")
                .CombineWithDefaultCulture()
                .WhereTrue("SKUEnabled")
                .OrderByDescending("SKUInStoreFrom")
                .ToList();
            
            // Displays the action's view with an initialized view model
            return View(products.Select(
                product => new ProductListItemViewModel(
                    product,
                    pricingService.CalculatePrice(product.SKU, shoppingService.GetCurrentShoppingCart()),
                    product.Product.PublicStatus?.PublicStatusDisplayName))
                );
        }
    }
    //EndDocSection:ListingController
}