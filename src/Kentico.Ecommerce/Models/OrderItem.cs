using System;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents an order item.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Original Kentico order item info object from which the model gathers information. See <see cref="OrderItemInfo"/> for detailed information.
        /// </summary>
        internal readonly OrderItemInfo OriginalOrderItem;


        /// <summary>
        /// Gets the path to the product image.
        /// </summary>
        public string ImagePath => OriginalOrderItem.OrderItemSKU.SKUImagePath;


        /// <summary>
        /// Gets the order item's identifier.
        /// </summary>
        public int ID => OriginalOrderItem.OrderItemID;


        /// <summary>
        /// Gets the total price of the order item in the main currency.
        /// </summary>
        public decimal Subtotal => OriginalOrderItem.OrderItemTotalPriceInMainCurrency;


        /// <summary>
        /// Gets the unit price of the order item.
        /// </summary>
        public decimal UnitPrice => OriginalOrderItem.OrderItemUnitPrice;


        /// <summary>
        /// Gets the number of the order items.
        /// </summary>
        public int Units => OriginalOrderItem.OrderItemUnitCount;


        /// <summary>
        /// Gets the order item's name.
        /// </summary>
        public string Name => OriginalOrderItem.OrderItemSKUName;


        /// <summary>
        /// Gets the SKU object's identifier.
        /// </summary>
        public int SKUID => OriginalOrderItem.OrderItemSKUID;


        /// <summary>
        /// Initializes a new instance of the <see cref="OrderItem"/> class.
        /// </summary>
        /// <param name="originalOrderItem"><see cref="OrderItemInfo"/> object representing an original Kentico order item info object from which the model is created.</param>
        public OrderItem(OrderItemInfo originalOrderItem)
        {
            if (originalOrderItem == null)
            {
                throw new ArgumentNullException(nameof(originalOrderItem));
            }

            OriginalOrderItem = originalOrderItem;
        }
    }
}
