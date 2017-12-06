using System;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents an item in a shopping cart.
    /// </summary>
    public class ShoppingCartItem
    {
        /// <summary>
        /// Original Kentico shopping cart item info object from which the model gathers information. See <see cref="ShoppingCartItemInfo"/> for detailed information.
        /// </summary>
        internal readonly ShoppingCartItemInfo OriginalCartItem;


        /// <summary>
        /// Gets the path to the product image.
        /// </summary>
        public string ImagePath => OriginalCartItem.SKU.SKUImagePath;


        /// <summary>
        /// Gets the shopping cart item identifier.
        /// </summary>
        public int ID => OriginalCartItem.CartItemID;


        /// <summary>
        /// Gets the total price of the shopping cart item in the shopping cart's currency.
        /// </summary>
        public decimal Subtotal => OriginalCartItem.TotalPrice;
        

        /// <summary>
        /// Gets the unit price of the shopping cart item.
        /// </summary>
        public decimal UnitPrice => OriginalCartItem.UnitPrice;


        /// <summary>
        /// Gets the number of the item units in the shopping cart.
        /// </summary>
        public int Units => OriginalCartItem.CartItemUnits;


        /// <summary>
        /// Gets the shopping cart item's name.
        /// </summary>
        public string Name => OriginalCartItem.SKU.SKUName;
        

        /// <summary>
        /// Gets the item's SKU object identifier.
        /// </summary>
        public int SKUID => OriginalCartItem.SKUID;


        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartItem"/> class.
        /// </summary>
        /// <param name="originalCartItem"><see cref="ShoppingCartItemInfo"/> object representing an original Kentico shopping cart item info object from which the model is created.</param>
        public ShoppingCartItem(ShoppingCartItemInfo originalCartItem)
        {
            if (originalCartItem == null)
            {
                throw new ArgumentNullException(nameof(originalCartItem));
            }

            OriginalCartItem = originalCartItem;
        }
    }
}
