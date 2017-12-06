using System;
using System.Linq;

using CMS.Core;
using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides methods for price calculation.
    /// </summary>
    public class PricingService : IPricingService
    {
        /// <summary>
        /// Calculates and returns prices of the given product.
        /// </summary>
        /// <param name="product">SKU info object (<see cref="SKUInfo"/>) of the product for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <returns><see cref="ProductPrice"/> object containing the product's prices.</returns>
        public virtual ProductPrice CalculatePrice(SKUInfo product, ShoppingCart cart)
        {
            if ((product == null) || (cart == null))
            {
                return null;
            }

            var originalCart = cart.OriginalCart;
            var cartCurrency = originalCart.Currency;

            var prices = Service.Resolve<ICatalogPriceCalculatorFactory>()
                .GetCalculator(originalCart.ShoppingCartSiteID)
                .GetPrices(product, Enumerable.Empty<SKUInfo>(), originalCart);

            var listPriceSource = Service.Resolve<ISKUPriceSourceFactory>().GetSKUListPriceSource(originalCart.ShoppingCartSiteID);
            var listPrice = listPriceSource.GetPrice(product, cartCurrency);

            // Return the calculated values
            return new ProductPrice
            {
                Currency = new Currency(cartCurrency),
                Discount = prices.StandardPrice - prices.Price,
                ListPrice = listPrice,
                Price = prices.Price,
                Tax = prices.Tax
            };
        }


        /// <summary>
        /// Calculates prices of the given product variant.
        /// </summary>
        /// <param name="variant">Variant model (<see cref="Variant"/>) of the variant's parent product for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <returns><see cref="ProductPrice"/> object containing the variant's prices.</returns>
        public virtual ProductPrice CalculatePrice(Variant variant, ShoppingCart cart)
        {
            return CalculatePrice(variant.VariantSKU, cart);
        }


        /// <summary>
        /// Calculates prices of the given shipping option.
        /// </summary>
        /// <param name="shippingInfo">Shipping option info object (<see cref="ShippingOptionInfo"/>) for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <returns><see cref="ShippingPrice"/> object containing the shipping's prices.</returns>
        public virtual ShippingPrice CalculateShippingOptionPrice(ShippingOptionInfo shippingInfo, ShoppingCart cart)
        {
            if (shippingInfo == null)
            {
                throw new ArgumentNullException(nameof(shippingInfo));
            }

            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            var originalCart = cart.OriginalCart;

            // Evaluate the cart to get total items price
            originalCart.Evaluate();

            var request = Service.Resolve<IShoppingCartAdapterService>().GetCalculationRequest(originalCart);
            request.ShippingOption = shippingInfo;
            var calculatorData = new CalculatorData(request, new CalculationResult());

            // Get shipping price
            var shippingPrice = Service.Resolve<IShippingPriceService>().GetShippingPrice(calculatorData, originalCart.TotalItemsPrice).Price;

            var roundingService = Service.Resolve<IRoundingServiceFactory>().GetRoundingService(originalCart.ShoppingCartSiteID);

            return new ShippingPrice
            {
                Currency = cart.Currency,
                Price = roundingService.Round(shippingPrice, cart.Currency.OriginalCurrency),
                Tax = 0m
            };
        }


        /// <summary>
        /// Calculates the remaining amount for free shipping.
        /// </summary>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the calculation information.</param>
        /// <returns>Returns the remaining amount for free shipping. Returns 0 if the cart is null, if there is no valid discount, or if free shipping was already applied.</returns>
        public decimal CalculateRemainingAmountForFreeShipping(ShoppingCart cart)
        {
            if ((cart == null) || cart.IsEmpty || !cart.IsShippingNeeded)
            {
                return 0m;
            }

            return cart.OriginalCart.CalculateRemainingAmountForFreeShipping();
        }
    }
}
