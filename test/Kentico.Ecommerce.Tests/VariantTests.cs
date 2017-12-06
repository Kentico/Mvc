using System.Linq;

using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    class ProductVariantTests : EcommerceTestsBase
    {
        private IVariantRepository variantRepository;

        private readonly string[] SIZES = { "XL", "XXL", "S" };
        private readonly string[] COLORS = { "Black", "White", "Even darker black" };
        private const string PATH_TO_VARIANT_IMAGE = "variant-image.png";
        private const string PATH_TO_PRODUCT_IMAGE = "parent-product.jpg";

        private SKUInfo skuWithVariants;
        private SKUInfo skuWithoutVariants;
        private OptionCategoryInfo categorySize;
        private OptionCategoryInfo categoryColor;
        private const int NONEXISTENT_SKU_ID = -1;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            InsertDefaultSite();

            variantRepository = new KenticoVariantRepository();

            SetUpOptionCategories();
            SetUpVariants();
        }


        [SetUp]
        public void SetUp()
        {
            SiteContext.CurrentSite = SiteInfo;
        }


        public void SetUpOptionCategories()
        {
            // Create categories
            categorySize = Factory.NewOptionCategory("Size").With(c => c.Insert());
            categoryColor = Factory.NewOptionCategory("Color").With(c => c.Insert());

            // Create parent product
            skuWithVariants = Factory.NewSKU("Hammer");
            skuWithVariants.SKUImagePath = PATH_TO_PRODUCT_IMAGE;
            skuWithVariants.Insert();

            // Create product without variants
            skuWithoutVariants = Factory.NewSKU("Hammer without nails").With(s => s.Insert());

            // Assign categories to product
            new SKUOptionCategoryInfo
            {
                AllowAllOptions = true,
                SKUID = skuWithVariants.SKUID,
                CategoryID = categorySize.CategoryID,
            }.Insert();

            new SKUOptionCategoryInfo
            {
                AllowAllOptions = true,
                SKUID = skuWithVariants.SKUID,
                CategoryID = categoryColor.CategoryID,
            }.Insert();

            new SKUOptionCategoryInfo
            {
                AllowAllOptions = true,
                SKUID = skuWithoutVariants.SKUID,
                CategoryID = categoryColor.CategoryID,
            }.Insert();
        }


        public void SetUpVariants()
        {
            var colors = COLORS.Select(colorName =>
            {
                var option = new SKUInfo
                {
                    SKUSiteID = SiteInfo.SiteID,
                    SKUPrice = 0,
                    SKUName = colorName,
                    SKUEnabled = true,
                    SKUOptionCategoryID = categorySize.CategoryID
                };
                option.Insert();
                return option;

            }).ToList();

            var sizes = SIZES.Select(sizeName =>
            {
                var option = new SKUInfo
                {
                    SKUSiteID = SiteInfo.SiteID,
                    SKUPrice = 0,
                    SKUName = sizeName,
                    SKUEnabled = true,
                    SKUOptionCategoryID = categoryColor.CategoryID
                };
                option.Insert();
                return option;

            }).ToList();

            var combinations = from s in sizes
                               from c in colors
                               select new { Size = s, Color = c };

            foreach (var comb in combinations)
            {
                var enabled = !comb.Color.SKUName.EqualsCSafe("White");

                var variant = new SKUInfo
                {
                    SKUSiteID = SiteInfo.SiteID,
                    SKUPrice = 10,
                    SKUName = $"Hammer ({comb.Color.SKUName}, {comb.Size.SKUName})",
                    SKUEnabled = enabled,
                    SKUParentSKUID = skuWithVariants.SKUID,
                    SKUImagePath = enabled ? PATH_TO_VARIANT_IMAGE : null
                };
                variant.Insert();

                new VariantOptionInfo
                {
                    VariantSKUID = variant.SKUID,
                    OptionSKUID = comb.Color.SKUID,
                }.Insert();
                new VariantOptionInfo
                {
                    VariantSKUID = variant.SKUID,
                    OptionSKUID = comb.Size.SKUID,
                }.Insert();
            }
        }


        [Test]
        public void GetProductVariants_ReturnAllVariants()
        {
            var variants = variantRepository.GetByProductId(skuWithVariants.SKUID).ToList();
            var firstVariant = variants.FirstOrDefault();
            var enabledVariants = variants.Where(v => v.Enabled);
            var disabledVariants = variants.Where(v => !v.Enabled);

            CMSAssert.All(
                () => Assert.AreEqual(SIZES.Length * COLORS.Length, variants.Count(), "Not all variants are returned."),
                () => Assert.AreEqual(2, firstVariant.ProductAttributes.Count(), "Variant has wrong option count"),
                () => Assert.IsTrue(firstVariant.ProductAttributes.Any(o => SIZES.Any(s => s.EqualsCSafe(o.SKUName))), "Variant do not contain correct option"),
                () => Assert.IsTrue(enabledVariants.Any(), "Enabled variants are not returned."),
                () => Assert.IsTrue(disabledVariants.Any(), "Disabled variants are not returned.")
            );
        }


        [Test]
        public void GetProductVariants_ReturnVariant()
        {
            var variants = variantRepository.GetByProductId(skuWithVariants.SKUID).ToList();
            var firstVariant = variants.FirstOrDefault();

            var variantID = firstVariant.VariantSKU.SKUID;
            var selectedVariant = variantRepository.GetById(variantID);

            CMSAssert.All(
                () => Assert.IsNotNull(selectedVariant, "Variant was not returned"),
                () => Assert.AreEqual(2, selectedVariant.ProductAttributes.Count(), "Variant has wrong option count"),
                () => Assert.IsTrue(selectedVariant.ProductAttributes.Any(o => SIZES.Any(s => s.EqualsCSafe(o.SKUName))), "Variant do not contain correct option")
            );
        }


        [Test]
        public void GetVariantOptionCategories_ReturnCategories()
        {
            var categories = variantRepository.GetVariantOptionCategories(skuWithVariants.SKUID);

            var colorCategory = categories.First(c => c.DisplayName.EqualsCSafe(categoryColor.CategoryDisplayName));

            CMSAssert.All(
                () => Assert.AreEqual(2, categories.Count(), "Variant option categories are not returned."),
                () => Assert.AreEqual(COLORS.Count(), colorCategory.CategoryOptions.Count(), "Category has wrong option count.")
            );
        }


        [Test]
        public void GetVariantOptionCategories_ExistingSKUWithoutCategories_ReturnsEmptyCollection()
        {
            var categories = variantRepository.GetVariantOptionCategories(skuWithoutVariants.SKUID);

            CMSAssert.All(
                () => Assert.IsNotNull(categories, "Returned null instead of an empty collection."),
                () => Assert.IsEmpty(categories, "Returned option categories for a product without variants.")
            );
        }


        [Test]
        public void GetVariantOptionCategories_NonExistentSKU_ReturnsEmptyCollection()
        {
            var categories = variantRepository.GetVariantOptionCategories(NONEXISTENT_SKU_ID);

            CMSAssert.All(
                () => Assert.IsNotNull(categories, "Returned null instead of an empty collection."),
                () => Assert.IsEmpty(categories, "Returned option categories for non-existent SKU")
            );
        }


        [Test]
        public void GetVariantFromOptions_ReturnVariant()
        {
            var variants = variantRepository.GetByProductId(skuWithVariants.SKUID).ToList();
            var firstVariant = variants.FirstOrDefault();

            var options = firstVariant.ProductAttributes.Select(o => o.SKUID).ToList();

            var selectedVariant = variantRepository.GetByProductIdAndOptions(skuWithVariants.SKUID, options);

            CMSAssert.All(
                () => Assert.NotNull(selectedVariant, "Variant for selected options was not returned."),
                () => Assert.AreEqual(selectedVariant.ProductAttributes.Count(), options.Count(), "Variant has wrong option count.")
            );
        }


        [Test]
        public void GetVariantImage_PathToImageReturned()
        {
            var variants = variantRepository.GetByProductId(skuWithVariants.SKUID).ToList();
            // Enabled variant has sku image path set
            var enabledVariant = variants.FirstOrDefault(v => v.Enabled);
            var disabledVariant = variants.FirstOrDefault(v => !v.Enabled);

            Assert.AreEqual(PATH_TO_VARIANT_IMAGE, enabledVariant.VariantSKU.SKUImagePath, "Variant image is not returned");
            Assert.AreEqual(PATH_TO_PRODUCT_IMAGE, disabledVariant.VariantSKU.SKUImagePath, "Parent product image is not returned");
        }
    }
}
