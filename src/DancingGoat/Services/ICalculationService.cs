using CMS.Ecommerce;

using Kentico.Core.DependencyInjection;
using Kentico.Ecommerce;

namespace DancingGoat.Services
{
    /// <summary>
    /// Interface for service providing methods for prices calculations.
    /// </summary>
    public interface ICalculationService : IService
    {
        /// <summary>
        /// Calculates product prices to be displayed on a listing page.
        /// </summary>
        /// <param name="product">Product to calculate prices for.</param>
        ProductPrice CalculateListingPrice(SKUInfo product);


        /// <summary>
        /// Calculates product prices to be displayed on product detail page.
        /// </summary>
        /// <param name="product">Product to calculate prices for.</param>
        ProductPrice CalculateDetailPrice(SKUInfo product);


        /// <summary>
        /// Calculates product variant prices to be displayed on product detail page.
        /// </summary>
        /// <param name="variant">Product variant to calculate prices for.</param>
        /// <returns></returns>
        ProductPrice CalculateDetailPrice(Variant variant);
    }
}