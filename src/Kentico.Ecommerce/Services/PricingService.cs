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
        /// <param name="applyDiscounts">Indicates if the prices are returned after applying catalog discounts.</param>
        /// <param name="applyTaxes">Indicates if the prices are returned after applying taxes.</param>
        /// <returns><see cref="ProductPrice"/> object containing the product's prices.</returns>
        public virtual ProductPrice CalculatePrice(SKUInfo product, ShoppingCart cart, bool applyDiscounts = true, bool applyTaxes = true)
        {
            if ((product == null) || (cart == null))
            {
                return null;
            }

            // If the product is a variant, get the parent product's SKU object for tax calculation
            var parent = product.IsProductVariant ? product.Parent as SKUInfo : product;
            if (parent == null)
            {
                return null;
            }

            // Get the product's price and list price without taxes
            var originalCart = cart.OriginalCart;
            var price = SKUInfoProvider.GetSKUPrice(product, originalCart, applyDiscounts, false);
            var listPrice = product.SKURetailPrice;

            // Calculate discounts and taxes if required
            var discount = applyDiscounts ? (SKUInfoProvider.GetSKUPrice(product, originalCart) - price) : 0.0;
            var tax = 0.0;

            if (applyTaxes)
            {
                // Calculate taxes and add them to the prices
                if (price > 0)
                {
                    tax = SKUInfoProvider.CalculateSKUTotalTax(parent, originalCart, price);
                    price += tax;
                }

                if (listPrice > 0)
                {
                    var listPriceTax = SKUInfoProvider.CalculateSKUTotalTax(parent, originalCart, listPrice);
                    listPrice += listPriceTax;
                } 
            }

            // Return the calculated values
            return new ProductPrice
            {
                Currency = new Currency(CurrencyInfoProvider.GetMainCurrency(parent.SKUSiteID)),
                Discount = (decimal) originalCart.RoundTo(discount),
                ListPrice = (decimal) originalCart.RoundTo(listPrice),
                Price = (decimal) originalCart.RoundTo(price),
                Tax = (decimal) originalCart.RoundTo(tax)
            };
        }


        /// <summary>
        /// Calculates prices of the given product variant.
        /// </summary>
        /// <param name="variant">Variant model (<see cref="Variant"/>) of the variant's parent product for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <param name="applyDiscounts">Indicates if the prices are returned after applying catalog discounts.</param>
        /// <param name="applyTaxes">Indicates if the prices are returned after applying taxes.</param>
        /// <returns><see cref="ProductPrice"/> object containing the variant's prices.</returns>
        public virtual ProductPrice CalculatePrice(Variant variant, ShoppingCart cart, bool applyDiscounts = true, bool applyTaxes = true)
        {
            return CalculatePrice(variant.VariantSKU, cart, applyDiscounts, applyTaxes);
        }


        /// <summary>
        /// Calculates prices of the given shipping option.
        /// </summary>
        /// <param name="shippingInfo">Shipping option info object (<see cref="ShippingOptionInfo"/>) for which the prices are calculated.</param>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) used to gather the price calculation information.</param>
        /// <param name="applyTaxes">Indicates if the prices are returned after applying taxes.</param>
        /// <returns><see cref="ShippingPrice"/> object containing the shipping's prices.</returns>
        public virtual ShippingPrice CalculateShippingOptionPrice(ShippingOptionInfo shippingInfo, ShoppingCart cart, bool applyTaxes = true)
        {
            var originalCart = cart.OriginalCart;

            lock (originalCart)
            {
                // Store the original shipping option ID 
                var origShippingOptionId = originalCart.ShoppingCartShippingOptionID;

                // Calculate a hypothetical shipping cost for the shipping option from the supplied list item
                originalCart.ShoppingCartShippingOptionID = shippingInfo.ShippingOptionID;

                var price = CalculateShippingOptionPrice(originalCart, applyTaxes);

                // Restore the original shipping option ID
                originalCart.ShoppingCartShippingOptionID = origShippingOptionId;

                return price;
            }
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
                return 0;
            }

            return (decimal)cart.OriginalCart.CalculateRemainingAmountForFreeShipping();
        }


        private ShippingPrice CalculateShippingOptionPrice(ShoppingCartInfo cartInfo, bool applyTaxes)
        {
            // Get the shipping cost for the currently processed shipping option
            var shippingPrice = cartInfo.Shipping;
            var tax = 0.0;

            if (applyTaxes)
            {
                tax = cartInfo.TotalShipping - shippingPrice;
                shippingPrice += tax;
            }

            return new ShippingPrice
            {
                Currency = new Currency(CurrencyInfoProvider.GetMainCurrency(cartInfo.ShoppingCartSiteID)),
                Price = (decimal)cartInfo.RoundTo(shippingPrice),
                Tax = (decimal)cartInfo.RoundTo(tax)
            };
        }
    }
}
