using System;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a product variant.
    /// </summary>
    public class Variant
    {
        /// <summary>
        /// Original Kentico variant's SKU info object from which the model gathers information. See <see cref="SKUInfo"/> for detailed information.
        /// </summary>
        internal readonly SKUInfo VariantSKU;


        /// <summary>
        /// Gets the product variant's SKU object identifier.
        /// </summary>
        public int VariantSKUID => VariantSKU.SKUID;


        /// <summary>
        /// Gets the product variant's name.
        /// </summary>
        public string Name => VariantSKU.SKUName;


        /// <summary>
        /// Gets the product variant's SKU number.
        /// </summary>
        public string SKUNumber => VariantSKU.SKUNumber;


        /// <summary>
        /// Gets the product variant's number of available items.
        /// </summary>
        /// <remarks>
        /// Number of available items from the variant's parent is returned in case of tracking by <see cref="TrackInventoryTypeEnum.ByProduct"/>.
        /// </remarks>
        public int AvailableItems => VariantSKU.SKUAvailableItems;


        /// <summary>
        /// Gets the product variant's price.
        /// </summary>
        /// <remarks>
        /// Catalog discounts or taxes are not included. Use <see cref="PricingService.CalculatePrice(Variant, ShoppingCart)"/> to return the final price. 
        /// </remarks>
        public decimal VariantPrice => VariantSKU.SKUPrice;


        /// <summary>
        /// Indicates if the product inventory is tracked. 
        /// <c>true</c> - inventory is tracked by the parent product or by variants.
        /// <c>false</c> - inventory tracking is disabled.
        /// </summary>
        public bool InventoryTracked => VariantSKU.SKUTrackInventory != TrackInventoryTypeEnum.Disabled;


        /// <summary>
        /// Indicates if the product variant is allowed for sale.
        /// </summary>
        public bool Enabled => VariantSKU.SKUEnabled;


        /// <summary>
        /// Gets the path to the product variant image.
        /// </summary>
        public string ImagePath => VariantSKU.SKUImagePath;


        /// <summary>
        /// Gets the product options of this variant.
        /// </summary>
        public ProductAttributeSet ProductAttributes
        {
            get;
        }


        /// <summary>
        /// Creates new instance of <see cref="Variant"/> class representing a product variant.
        /// </summary>
        /// <param name="sku"><see cref="SKUInfo"/> object representing an original Kentico variant's SKU info object from which the model is created.</param>
        /// <param name="attributes"><see cref="ProductAttributeSet"/> object representing the product options of this variant.</param>
        public Variant(SKUInfo sku, ProductAttributeSet attributes)
        {
            if (sku == null)
            {
                throw new ArgumentNullException(nameof(sku));
            }
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            VariantSKU = sku;
            ProductAttributes = attributes;
        }
    }
}
