using CMS.Ecommerce;

using Kentico.Ecommerce;

namespace DancingGoat.Services
{
    /// <summary>
    /// Provides methods for prices calculations.
    /// </summary>
    public class CalculationService : ICalculationService
    {
        private readonly IShoppingService mShoppingService;
        private readonly IPricingService mPricingService;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shoppingService">Shopping service.</param>
        /// <param name="pricingService">Pricing service.</param>
        public CalculationService(IShoppingService shoppingService, IPricingService pricingService)
        {
            mShoppingService = shoppingService;
            mPricingService = pricingService;
        }


        /// <summary>
        /// Calculates product prices to be displayed on a listing page.
        /// </summary>
        /// <param name="product">Product to calculate prices for.</param>
        public ProductPrice CalculateListingPrice(SKUInfo product)
        {
            return mPricingService.CalculatePrice(product, mShoppingService.GetCurrentShoppingCart());
        }


        /// <summary>
        /// Calculates product prices to be displayed on product detail page.
        /// </summary>
        /// <param name="product">Product to calculate prices for.</param>
        public ProductPrice CalculateDetailPrice(SKUInfo product)
        {
            return mPricingService.CalculatePrice(product, mShoppingService.GetCurrentShoppingCart());
        }


        /// <summary>
        /// Calculates product variant prices to be displayed on product detail page.
        /// </summary>
        /// <param name="variant">Product variant to calculate prices for.</param>
        /// <returns></returns>
        public ProductPrice CalculateDetailPrice(Variant variant)
        {
            return mPricingService.CalculatePrice(variant, mShoppingService.GetCurrentShoppingCart());
        }
    }
}