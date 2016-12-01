namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents information about product prices and their components.
    /// </summary>
    public sealed class ProductPrice : PriceBase
    {
        /// <summary>
        /// Amount of catalog discounts.
        /// </summary>
        public decimal Discount
        {
            get;
            set;
        }


        /// <summary>
        /// Base list price of the product.
        /// </summary>
        public decimal ListPrice
        {
            get;
            set;
        }
    }
}
