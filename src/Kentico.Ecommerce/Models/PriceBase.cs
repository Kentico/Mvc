using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a calculated price with or without taxes. If the price is calculated without taxes, the <see cref="Tax"/> property contains 0.
    /// </summary>
    public abstract class PriceBase
    {
        /// <summary>
        /// Currency in which the price and tax are expressed.
        /// </summary>
        public Currency Currency
        {
            get;
            set;
        }


        /// <summary>
        /// Calculated price including taxes (if calculated with them).
        /// </summary>
        public decimal Price
        {
            get;
            set;
        }


        /// <summary>
        /// Price's tax amount. If the price is calculated without taxes, it contains 0.
        /// </summary>
        public decimal Tax
        {
            get;
            set;
        }
    }
}
