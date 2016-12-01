using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Ecommerce;

using LearningKit.Models.ProductFilter;
using LearningKit.Models.Products;

namespace LearningKit.Controllers
{
    public class ProductFilterController : Controller
    {
        private readonly IShoppingService shoppingService;
        private readonly IPricingService pricingService;

        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the services.
        /// </summary>
        public ProductFilterController()
        {
            //DocSection:ServiceInit
            shoppingService = new ShoppingService();
            pricingService = new PricingService();
            //EndDocSection:ServiceInit
        }

        /// <summary>
        /// Displays a product listing page of the class's product page type with a possibility
        /// to filter products based on an simple page type information.
        /// </summary>
        public ActionResult FilterPageProperty()
        {
            ProductFilterViewModel model = new ProductFilterViewModel
            {
                LPTWithFeature = false,
                FilteredProducts = LoadProducts()
            };

            return View(model);
        }

        /// <summary>
        /// Displays a product listing page of the class's product page type
        /// filtered based on the specified model.
        /// </summary>
        /// <param name="model">Model specifying all filtered products.</param>
        [HttpPost]
        public ActionResult FilterPageProperty(ProductFilterViewModel model)
        {
            //DocSection:PagePropertyModel
            // Creates a view model that consists of information
            // whether the 'LPTWithFeature' is selected and a list of products
            ProductFilterViewModel filteredModel = new ProductFilterViewModel
            {
                LPTWithFeature = model.LPTWithFeature,
                FilteredProducts = LoadProducts(GetWithFeatureWhereCondition(model))
            };
            
            return View(filteredModel);
            //EndDocSection:PagePropertyModel
        }

        /// <summary>
        /// Returns a where condition to correctly retrieve which products are selected in the filter.
        /// </summary>
        /// <param name="model">Model specifying all filtered products.</param>
        /// <returns>Where condition specifying which products are selected.</returns>
        private WhereCondition GetWithFeatureWhereCondition(ProductFilterViewModel model)
        {
            //DocSection:PagePropertyWhere
            // Initializes a new where condition
            WhereCondition withFeatureWhere = new WhereCondition();
            
            // If the feature is selected, sets the where condition
            if (model.LPTWithFeature)
            {
                withFeatureWhere.WhereTrue("LPTWithFeature");
            }
            //EndDocSection:PagePropertyWhere

            // Returns the where condition
            return withFeatureWhere;
        }

        /// <summary>
        /// Displays a product listing page of the class's product page type with a possibility
        /// to filter products based on an simple SKU information.
        /// </summary>
        public ActionResult FilterSKUProperty()
        {
            ProductFilterViewModel model = new ProductFilterViewModel
            {
                FilteredProducts = LoadProducts()
            };

            return View(model);
        }

        /// <summary>
        /// Displays a product listing page of the class's product page type
        /// filtered based on the specified model.
        /// </summary>
        /// <param name="model">Model specifying all filtered products.</param>
        [HttpPost]
        public ActionResult FilterSKUProperty(ProductFilterViewModel model)
        {
            //DocSection:SKUPropertyModel
            // Creates a view model that consists of the entered price range
            // and a list of products
            ProductFilterViewModel filteredModel = new ProductFilterViewModel
            {
                PriceFrom = model.PriceFrom,
                PriceTo = model.PriceTo,
                FilteredProducts = LoadProducts(GetPriceWhereCondition(model))
            };
            
            return View(filteredModel);
            //EndDocSection:SKUPropertyModel
        }

        /// <summary>
        /// Returns a where condition to correctly retrieve which products are selected in the filter.
        /// </summary>
        /// <param name="model">Model specifying all filtered products.</param>
        /// <returns>Where condition specifying which products are selected.</returns>
        private WhereCondition GetPriceWhereCondition(ProductFilterViewModel model)
        {
            //DocSection:SKUPropertyWhere
            // Initializes a new where condition
            WhereCondition priceWhere = new WhereCondition();
            
            // Sets the price where condition based on the model's values and limited by the price from-to range
            if (Constrain(model.PriceFrom, model.PriceTo))
            {
                priceWhere.WhereGreaterOrEquals("SKUPrice", model.PriceFrom)
                    .And().WhereLessOrEquals("SKUPrice", model.PriceTo);
            }
            //EndDocSection:SKUPropertyWhere

            // Returns the where condition
            return priceWhere;
        }

        /// <summary>
        /// Dummy check that the from and to are not zero to provide a working example.
        /// </summary>
        /// <param name="from">Price from.</param>
        /// <param name="to">Price true.</param>
        /// <returns>True if at least one of the parameters is not 0.</returns>
        private bool Constrain(decimal from, decimal to)
        {
            return (from != 0) || to != 0;
        }

        /// <summary>
        /// Displays a product listing page of the class's product page type with a possibility
        /// to filter products based on a foreign entity.
        /// </summary>
        public ActionResult FilterForeignProperty()
        {
            //DocSection:ForeignPropertyGetModel
            // Creates a view model that consists of all foreign objects (manufacturers) related to the products
            // and a list of products that will be filtered
            ProductFilterViewModel model = new ProductFilterViewModel
            {
                Manufacturers = GetManufacturers(),
                FilteredProducts = LoadProducts()
            };
            
            return View(model);
            //EndDocSection:ForeignPropertyGetModel
        }

        /// <summary>
        /// Displays a product listing page of the class's product page type
        /// filtered based on the specified model.
        /// </summary>
        /// <param name="model">Model specifying all foreign objects and all filtered products.</param>
        [HttpPost]
        public ActionResult FilterForeignProperty(ProductFilterViewModel model)
        {
            //DocSection:ForeignPropertyPostModel
            // Creates a view model that consists of all foreign objects (manufacturers) related to the products
            // and a list of products that will be filtered with their selected state
            ProductFilterViewModel filteredModel = new ProductFilterViewModel
            {
                Manufacturers = model.Manufacturers,
                FilteredProducts = LoadProducts(GetManufacturersWhereCondition(model))
            };
            
            return View(filteredModel);
            //EndDocSection:ForeignPropertyPostModel
        }

        /// <summary>
        /// Loads all available manufacturers (as unselected) used for products of the LearningProductType product page type.
        /// </summary>
        /// <returns>List of manufacturers' models and their unselected state.</returns>
        private List<ProductFilterCheckboxViewModel> GetManufacturers()
        {
            // Gets manufacturers of all the product type's products
            var manufacturers = LearningProductTypeProvider.GetLearningProductTypes()
                .ToList()
                .Where(skuPage => skuPage.Product.Manufacturer != null)
                .Select(skuPage =>
                    new
                    {
                        skuPage.Product.Manufacturer?.ManufacturerID,
                        skuPage.Product.Manufacturer?.ManufacturerDisplayName
                    })
                .Distinct();

            // Returns a model that contains the manufacturers' display name, ID and false select state
            return manufacturers.Select(manufacturer => new ProductFilterCheckboxViewModel
            {
                DisplayName = manufacturer.ManufacturerDisplayName,
                Value = manufacturer.ManufacturerID.ToString(),
                IsChecked = false
            }).ToList();
        }

        /// <summary>
        /// Returns a where condition to correctly retrieve which manufacturers are selected in the filter.
        /// </summary>
        /// <param name="model">Model specifying all foreign objects and all filtered products.</param>
        /// <returns>Manufacturers selected in the filter.</returns>
        private WhereCondition GetManufacturersWhereCondition(ProductFilterViewModel model)
        {
            //DocSection:ForeignPropertyWhere
            // Initializes a new where condition
            WhereCondition manufacturersWhere = new WhereCondition();
            
            // Gets a list of strings representing display names of selected manufacturers
            List<string> selectedManufacturersIds = GetSelectedManufacturersIds(model);
            
            // If any manufacturer is selected, sets the where condition
            if (selectedManufacturersIds.Any())
            {
                manufacturersWhere.WhereIn("SKUManufacturerID", selectedManufacturersIds);
            }
            //EndDocSection:ForeignPropertyWhere

            // Returns the where condition
            return manufacturersWhere;
        }

        /// <summary>
        /// Returns a list of code names of the selected manufacturers in the specified model.
        /// </summary>
        /// <param name="model">Model specifying all foreign objects and all filtered products.</param>
        /// <returns>List of code names of the selected manufacturers.</returns>
        private List<string> GetSelectedManufacturersIds(ProductFilterViewModel model)
        {
            return model.Manufacturers
                .Where(manufacturer => manufacturer.IsChecked)
                .Select(manufacturer => manufacturer.Value)
                .ToList();
        }

        /// <summary>
        /// Loads pages from the current site of the LearningProductType product page type.
        /// </summary>
        /// <returns>List of view models representing a products, its prices and public status display name.</returns>
        private List<ProductListItemViewModel> LoadProducts()
        {
            return LoadProducts(null);
        }

        /// <summary>
        /// Loads pages from the current site of the LearningProductType product page type based on the specified where condition.
        /// </summary>
        /// <param name="where">Where condition that restricts the returned products.</param>
        /// <returns>List of view models representing a products, its prices and public status display name.</returns>
        private List<ProductListItemViewModel> LoadProducts(WhereCondition where)
        {
            // Gets products of the product page type
            List<LearningProductType> products = LearningProductTypeProvider.GetLearningProductTypes()
               .LatestVersion(false)
               .Published(true)
               .OnSite(SiteContext.CurrentSiteName)
               .Culture("en-US")
               .CombineWithDefaultCulture()
               .WhereTrue("SKUEnabled")
               .Where(where)
               .OrderByDescending("SKUInStoreFrom")
               .ToList();

            // Displays an initialized view model
            return products.Select(
                product => new ProductListItemViewModel(
                    product,
                    pricingService.CalculatePrice(product.SKU, shoppingService.GetCurrentShoppingCart()),
                    product.Product.PublicStatus?.PublicStatusDisplayName)
                ).ToList();
        }
    }
}