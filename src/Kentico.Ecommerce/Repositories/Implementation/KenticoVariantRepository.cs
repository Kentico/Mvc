using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.SiteProvider;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides CRUD operations for product variants.
    /// </summary>
    public class KenticoVariantRepository : IVariantRepository
    {
        private int SiteID
        {
            get
            {
                return SiteContext.CurrentSiteID;
            }
        }


        /// <summary>
        /// Returns an enumerable collection of all variants of the given product.
        /// </summary>
        /// <param name="productId">SKU object identifier of the variant's parent product.</param>
        /// <returns>Collection of product variants. See <see cref="Variant"/> for detailed information.</returns>
        public IEnumerable<Variant> GetByProductId(int productId)
        {
            var variantSKUs = VariantHelper.GetVariants(productId).OnSite(SiteID).ToList();

            // Get the used option IDs for the variants
            var variantOptions = VariantOptionInfoProvider.GetVariantOptions()
                .WhereIn("VariantSKUID", variantSKUs.Select(s => s.SKUID).ToList())
                .ToList();

            // Pre-load option SKUs for the variants
            var options = SKUInfoProvider.GetSKUs()
                .WhereIn("SKUID", variantOptions.Select(v => v.OptionSKUID).ToList())
                .ToList();

            // Create variants with the options
            return variantSKUs.Select(sku => new Variant(
                sku,
                new ProductAttributeSet(variantOptions
                    .Where(o => o.VariantSKUID == sku.SKUID)
                    .Select(o => options.First(s => s.SKUID == o.OptionSKUID))
                    .ToArray())
            ));
        }


        /// <summary>
        /// Returns a variant with the specified identifier.
        /// </summary>
        /// <param name="variantId">Product variant's SKU object identifier.</param>
        /// <returns><see cref="Variant"/> object representing a product variant with the specified identifier. Returns <c>null</c> if not found.</returns>
        public Variant GetById(int variantId)
        {
            var variantSKU = SKUInfoProvider.GetSKUInfo(variantId);

            if ((variantSKU == null) || (SiteID != variantSKU.SKUSiteID))
            {
                return null;
            }

            return GetVariant(variantSKU);
        }


        /// <summary>
        /// Returns a collection of option categories used in a product's variants.
        /// </summary>
        /// <param name="productId">SKU identifier of the variant's parent product.</param>
        /// <returns>Collection of option categories used in a product's variants. See <see cref="ProductOptionCategory"/> for detailed information.</returns>
        public IEnumerable<ProductOptionCategory> GetVariantOptionCategories(int productId)
        {
            // Get a list of option categories
            var optionCategoriesList = VariantHelper.GetProductVariantsCategories(productId, false).ToList();

            // Get all variant's options
            var variantOptionIDs = VariantOptionInfoProvider.GetVariantOptions()
                .WhereIn("VariantSKUID", VariantHelper.GetVariants(productId).Column("SKUID"))
                .Column("OptionSKUID");

            var variantOptionsList = SKUInfoProvider.GetSKUs()
                .WhereIn("SKUID", variantOptionIDs)
                .OrderBy("SKUOrder")
                .ToList();

            // Create option categories with selectable variant options
            return optionCategoriesList.Select(cat =>
                new ProductOptionCategory(
                    cat,
                    variantOptionsList.Where(o => o.SKUOptionCategoryID == cat.CategoryID)
                )
            );
        }


        /// <summary>
        /// Returns a variant for the given parent product which consists of the specified options.
        /// If multiple variants use the given subset of options, one of them is returned (based on setting of the database engine).
        /// </summary>
        /// <param name="productId">SKU identifier of the variant's parent product.</param>
        /// <param name="optionIds">Collection of the variant's product options.</param>
        /// <returns><see cref="Variant"/> object representing a product variant assembled from the specified information. Returns <c>null</c> if such variant does not exist.</returns>
        public Variant GetByProductIdAndOptions(int productId, IEnumerable<int> optionIds)
        {
            var variantSKU = VariantHelper.GetProductVariant(productId, new ProductAttributeSet(optionIds));

            if (variantSKU == null)
            {
                return null;
            }

            return GetVariant(variantSKU);
        }


        private Variant GetVariant(SKUInfo variantSKU)
        {
            // Get product options' SKUs for the variant
            var options = SKUInfoProvider.GetSKUs()
                .WhereIn("SKUID", new IDQuery<VariantOptionInfo>("OptionSKUID")
                    .WhereEquals("VariantSKUID", variantSKU.SKUID)
            ).ToArray();

            return new Variant(variantSKU, new ProductAttributeSet(options));
        }
    }
}
