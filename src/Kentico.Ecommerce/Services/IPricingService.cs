using CMS.Ecommerce;

using Kentico.Core.DependencyInjection;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Interface for a service providing methods for price calculation.
    /// </summary>
    public interface IPricingService : IService
    {
        /// <summary>
        /// Calculates and returns prices of the given product.
        /// </summary>
        /// <param name="product">SKU info object (<see cref="SKUInfo"/>) of the product for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <param name="applyDiscounts">Indicates if the prices are returned after applying catalog discounts.</param>
        /// <param name="applyTaxes">Indicates if the prices are returned after applying taxes.</param>
        /// <returns><see cref="ProductPrice"/> object containing the product's prices.</returns>
        ProductPrice CalculatePrice(SKUInfo product, ShoppingCart cart, bool applyDiscounts = true, bool applyTaxes = true);


        /// <summary>
        /// Calculates prices of the given product variant.
        /// </summary>
        /// <param name="variant">Variant model (<see cref="Variant"/>) of the variant's parent product for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <param name="applyDiscounts">Indicates if the prices are returned after applying catalog discounts.</param>
        /// <param name="applyTaxes">Indicates if the prices are returned after applying taxes.</param>
        /// <returns><see cref="ProductPrice"/> object containing the variant's prices.</returns>
        ProductPrice CalculatePrice(Variant variant, ShoppingCart cart, bool applyDiscounts = true, bool applyTaxes = true);


        /// <summary>
        /// Calculates prices of the given shipping option.
        /// </summary>
        /// <param name="shippingInfo">Shipping option info object (<see cref="ShippingOptionInfo"/>) for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <param name="applyTaxes">Indicates if the prices are returned after applying taxes.</param>
        /// <returns><see cref="ShippingPrice"/> object containing the shipping's prices.</returns>
        ShippingPrice CalculateShippingOptionPrice(ShippingOptionInfo shippingInfo, ShoppingCart cart, bool applyTaxes = true);


        /// <summary>
        /// Calculates the remaining amount for free shipping.
        /// </summary>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the calculation information.</param>
        /// <returns>Returns the remaining amount for free shipping. Returns 0 if the cart is null, if there is no valid discount, or if free shipping was already applied.</returns>
        decimal CalculateRemainingAmountForFreeShipping(ShoppingCart cart);
    }
}