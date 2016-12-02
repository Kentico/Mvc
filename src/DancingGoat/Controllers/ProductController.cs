using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Helpers;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Services;
using Kentico.Ecommerce;

namespace DancingGoat.Controllers
{
    public class ProductController : Controller
    {
        private readonly ICalculationService mCalculationService;
        private readonly IProductRepository mProductRepository;
        private readonly IVariantRepository mVariantRepository;
        private readonly TypedProductViewModelFactory mTypedProductViewModelFactory;


        public ProductController(ICalculationService calculationService, IProductRepository productRepository,
            IVariantRepository variantRepository, TypedProductViewModelFactory typedProductViewModelFactory)
        {
            mCalculationService = calculationService;
            mProductRepository = productRepository;
            mVariantRepository = variantRepository;
            mTypedProductViewModelFactory = typedProductViewModelFactory;
        }


        public ActionResult Detail(int id, string productAlias)
        {
            var product = mProductRepository.GetProduct(id);
            var sku = product?.SKU;

            // If a product is not found or not allowed for sale, redirect to 404
            if ((product == null) || (sku == null) || !sku.SKUEnabled)
            {
                return HttpNotFound();
            }

            // Redirect if the page alias does not match
            if (!string.IsNullOrEmpty(productAlias) &&
                !product.NodeAlias.Equals(productAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Detail", new { id = product.NodeID, productAlias = product.NodeAlias });
            }

            var viewModel = PrepareProductDetailViewModel(product);

            return View(viewModel);
        }


        [HttpPost]
        public JsonResult Variant(List<int> options, int parentProductID)
        {
            var variant = mVariantRepository.GetByProductIdAndOptions(parentProductID, options);

            if (variant == null)
            {
                return Json(new
                {
                    stockMessage = ResHelper.GetString("DancingGoatMvc.Product.NotAvailable"),
                    totalPrice ="-"
                });
            }

            var variantPrice = mCalculationService.CalculateDetailPrice(variant);
            var isInStock = !variant.InventoryTracked || variant.AvailableItems > 0;

            return GetVariantResponse(variantPrice,
                                      isInStock,
                                      isInStock ? "DancingGoatMvc.Product.InStock" : "DancingGoatMvc.Product.OutOfStock",
                                      variant.VariantSKUID);

        }


        private JsonResult GetVariantResponse(ProductPrice priceDetail, bool inStock, string stockMessageResourceString, int variantSKUID)
        {
            string priceSavings = string.Empty;

            var currency = priceDetail.Currency;
            var beforeDiscount = priceDetail.Price + priceDetail.Discount;

            if (priceDetail.Discount > 0)
            {
                var discountPercentage = Math.Round(priceDetail.Discount * 100 / beforeDiscount);
                var discount = currency.FormatPrice(priceDetail.Discount);
                priceSavings = $"{discount} ({discountPercentage}%)";
            }

            var response = new
            {
                totalPrice = currency.FormatPrice(priceDetail.Price),
                beforeDiscount = priceDetail.Discount > 0 ? currency.FormatPrice(beforeDiscount) : string.Empty,
                savings = priceSavings,
                stockMessage = ResHelper.GetString(stockMessageResourceString),
                inStock,
                variantSKUID
            };

            return Json(response);
        }


        private ProductViewModel PrepareProductDetailViewModel(SKUTreeNode product)
        {
            var sku = product.SKU;

            var cheapestVariant = GetCheapestVariant(product);
            var variantCategories = PrepareProductOptionCategoryViewModels(sku, cheapestVariant);

            // Calculate the price of the product or selected variant
            var price = cheapestVariant != null
                ? mCalculationService.CalculateDetailPrice(cheapestVariant)
                : mCalculationService.CalculateDetailPrice(sku);

            // Create a strongly typed view model with product type specific information
            var typedProduct = mTypedProductViewModelFactory.GetViewModel(product);

            var viewModel = (cheapestVariant != null)
                ? new ProductViewModel(product, price, typedProduct, cheapestVariant, variantCategories)
                : new ProductViewModel(product, price, typedProduct);
            return viewModel;
        }


        private IEnumerable<ProductOptionCategoryViewModel> PrepareProductOptionCategoryViewModels(SKUInfo sku, Variant cheapestVariant)
        {
            var categories = mVariantRepository.GetVariantOptionCategories(sku.SKUID);

            // Set the selected options in the variant categories which represents the cheapest variant
            var variantCategories =
                cheapestVariant?.ProductAttributes.Select(
                    option =>
                        new ProductOptionCategoryViewModel(option.SKUID,
                            categories.FirstOrDefault(c => c.ID == option.SKUOptionCategoryID)));

            return variantCategories;
        }


        private Variant GetCheapestVariant(SKUTreeNode product)
        {
            var variants = mVariantRepository.GetByProductId(product.NodeSKUID).OrderBy(v => v.VariantPrice).ToList();
            var cheapestVariant = variants.FirstOrDefault();

            return cheapestVariant;
        }
    }
}