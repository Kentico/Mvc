using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a currency of prices and provides a formatting method for the price.
    /// </summary>
    public class Currency
    {
        internal CurrencyInfo OriginalCurrency;


        /// <summary>
        /// Standard three-letter currency code.
        /// </summary>
        public string CurrencyCode => OriginalCurrency.CurrencyCode;


        /// <summary>
        /// Initializes a new instance of the <see cref="Currency"/> class.
        /// </summary>
        /// <param name="currency"><see cref="CurrencyInfo"/> object representing an original Kentico currency info object from which the model is created</param>
        public Currency(CurrencyInfo currency)
        {
            OriginalCurrency = currency;
        }


        /// <summary>
        /// Returns a string with the formatted price according to the original format string (<see cref="CurrencyInfo.CurrencyFormatString"/>).
        /// </summary>
        /// <param name="price">Price amount to be formatted.</param>
        public string FormatPrice(decimal price)
        {
            return CurrencyInfoProvider.GetFormattedPrice(price, OriginalCurrency, false);
        }
    }
}
