using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    class KenticoVariantRepositoryTests : UnitTests
    {
        private KenticoVariantRepository mRepository;

        private const int SITE_ID1 = 1;
        private const int SITE_ID2 = 2;

        private const int CATEGORY_ID1 = 1;
        private const int CATEGORY_ID2 = 2;
        private const int CATEGORY_ID3 = 3;

        private const int OPTION1_CAT1_ID = 1;
        private const int OPTION2_CAT1_ID = 2;
        private const int OPTION1_CAT2_ID = 3;
        private const int OPTION2_CAT2_ID = 4;
        private const int NONEXISTENT_OPTION_ID = 5;

        private const int PARENT_SKU_WITHOUTVARIANTS = 1;
        private const int PARENT_SKU_WITHVARIANTS = 2;
        private const int VARIANT_SKU_ID1 = 3;
        private const int VARIANT_SKU_ID2 = 4;
        private const int VARIANT_SKU_ID3 = 5;
        private const int VARIANT_CROSSSITE_ID = 6;
        private const int NONEXISTENT_SKU_ID = 7;


        [SetUp]
        public void SetUp()
        {
            SetUpSite();
            SetUpRelationShips();
            SetUpOptionsCategories();
            SetUpSKUs();

            SiteContext.CurrentSite = SiteInfoProvider.GetSiteInfo(SITE_ID1);

            mRepository = new KenticoVariantRepository();
        }


        [Test]
        public void GetProductVariant_ExistingEnabledVariant_ReturnsVariant()
        {
            var variant = mRepository.GetById(VARIANT_SKU_ID1);

            CMSAssert.All(
                () => Assert.IsNotNull(variant, "No object returned for an existing enabled variant."),
                () => Assert.AreEqual(VARIANT_SKU_ID1, variant.VariantSKUID)
            );
        }


        [Test]
        public void GetProductVariant_ExistingDisabledVariant_ReturnsVariant()
        {
            var variant = mRepository.GetById(VARIANT_SKU_ID3);

            Assert.AreEqual(VARIANT_SKU_ID3, variant.VariantSKUID);
        }


        [Test]
        public void GetProductVariant_NonExistentVariant_ReturnsNull()
        {
            var variant = mRepository.GetById(NONEXISTENT_SKU_ID);

            Assert.IsNull(variant, "Returned variant for non-existent SKUID.");
        }


        [Test]
        public void GetProductVariant_CrossSiteVariant_ReturnsNull()
        {
            var variant = mRepository.GetById(VARIANT_CROSSSITE_ID);

            Assert.IsNull(variant, "Returned variant from another site.");
        }


        [Test]
        public void GetProductVariants_ExistingSKUWithVariants_ReturnsAllVariants()
        {
            var variants = mRepository.GetByProductId(PARENT_SKU_WITHVARIANTS);

            CMSAssert.All(
                () => Assert.IsNotNull(variants, "Returned null for product with variants."),
                () => Assert.AreEqual(3, variants.Count(), "Returned incorrect number of variants.")
            );
        }


        [Test]
        public void GetProductVariants_ExistingSKUWithoutVariants_ReturnsEmptyCollection()
        {
            var variants = mRepository.GetByProductId(PARENT_SKU_WITHOUTVARIANTS);

            CMSAssert.All(
                () => Assert.IsNotNull(variants, "Returned null instead of an empty collection."),
                () => Assert.IsEmpty(variants, "Returned variants for a product without variant")
            );
        }


        [Test]
        public void GetProductVariants_NonExistentSKU_ReturnsEmptyCollection()
        {
            var variants = mRepository.GetByProductId(NONEXISTENT_SKU_ID);

            CMSAssert.All(
                () => Assert.IsNotNull(variants, "Returned null instead of an empty collection."),
                () => Assert.IsEmpty(variants, "Returned variants for a product without variant")
            );
        }


        [Test]
        public void GetVariantWithOptions_SpecifyingExactVariant_ReturnsVariant()
        {
            var options = new List<int>();
            options.Add(OPTION1_CAT1_ID);
            options.Add(OPTION1_CAT2_ID);

            var variant = mRepository.GetByProductIdAndOptions(PARENT_SKU_WITHVARIANTS, options);

            CMSAssert.All(
                () => Assert.IsNotNull(variant, "Returned null instead of a specific variant."),
                () => Assert.AreEqual(VARIANT_SKU_ID1, variant.VariantSKUID, "Returned incorrect variant.")
            );
        }


        [Test]
        public void GetVariantWithOptions_SpecifyingMoreVariants_ReturnsAnyVariant()
        {
            var options = new List<int>();
            options.Add(OPTION1_CAT1_ID);

            var variant = mRepository.GetByProductIdAndOptions(PARENT_SKU_WITHVARIANTS, options);

            Assert.IsNotNull(variant, "Returned null instead of a variant.");
        }


        [Test]
        public void GetVariantWithOptions_SpecifyingNonExistentVariant_ReturnsNull()
        {
            var options = new List<int>();
            options.Add(OPTION2_CAT1_ID);
            options.Add(OPTION2_CAT2_ID);

            var variant = mRepository.GetByProductIdAndOptions(PARENT_SKU_WITHVARIANTS, options);

            Assert.IsNull(variant, "Returned a variant for nonexistent combination of options.");
        }


        [Test]
        public void GetVariantWithOptions_UsingNonExistentParentSKU_ReturnsNull()
        {
            var options = new List<int>();
            options.Add(OPTION1_CAT1_ID);

            var variant = mRepository.GetByProductIdAndOptions(NONEXISTENT_SKU_ID, options);

            Assert.IsNull(variant, "Returned a variant for nonexistent parent SKU");
        }


        [Test]
        public void GetVariantWithOptions_UsingParentSKUWithoutVariants_ReturnsNull()
        {
            var options = new List<int>();
            options.Add(OPTION1_CAT1_ID);

            var variant = mRepository.GetByProductIdAndOptions(PARENT_SKU_WITHOUTVARIANTS, options);

            Assert.IsNull(variant, "Returned a variant for nonexistent parent SKU");
        }


        [Test]
        public void GetVariantWithOptions_UsingNonExistentOption_ReturnsNull()
        {
            var options = new List<int>();
            options.Add(NONEXISTENT_OPTION_ID);

            var variant = mRepository.GetByProductIdAndOptions(PARENT_SKU_WITHVARIANTS, options);

            Assert.IsNull(variant, "Returned variant for nonexistent product option.");
        }


        private void SetUpSite()
        {
            Fake<SiteInfo, SiteInfoProvider>().WithData(
               new SiteInfo
               {
                   SiteName = "testSite",
                   SiteID = SITE_ID1
               });
        }


        private void SetUpOptionsCategories()
        {
            Fake<OptionCategoryInfo, OptionCategoryInfoProvider>().WithData(
                new OptionCategoryInfo
                {
                    CategoryID = CATEGORY_ID1,
                    CategorySiteID = SITE_ID1,
                    CategoryEnabled = true
                },
                new OptionCategoryInfo
                {
                    CategoryID = CATEGORY_ID2,
                    CategorySiteID = SITE_ID1,
                    CategoryEnabled = true
                },
                new OptionCategoryInfo
                {
                    CategoryID = CATEGORY_ID3,
                    CategorySiteID = SITE_ID1,
                    CategoryEnabled = true
                });

            Fake<SKUInfo, SKUInfoProvider>().WithData(
                new SKUInfo
                {
                    SKUID = OPTION1_CAT1_ID,
                    SKUSiteID = SITE_ID1,
                    SKUOptionCategoryID = CATEGORY_ID1,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = OPTION2_CAT1_ID,
                    SKUSiteID = SITE_ID1,
                    SKUOptionCategoryID = CATEGORY_ID1,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = OPTION1_CAT2_ID,
                    SKUSiteID = SITE_ID1,
                    SKUOptionCategoryID = CATEGORY_ID1,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = OPTION2_CAT2_ID,
                    SKUSiteID = SITE_ID1,
                    SKUOptionCategoryID = CATEGORY_ID1,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                });
        }


        private void SetUpRelationShips()
        {
            Fake<SKUOptionCategoryInfo, SKUOptionCategoryInfoProvider>().WithData(
                new SKUOptionCategoryInfo
                {
                    SKUID = PARENT_SKU_WITHVARIANTS,
                    CategoryID = CATEGORY_ID1,
                    SKUCategoryID = 1,
                    AllowAllOptions = true

                },
                new SKUOptionCategoryInfo
                {
                    SKUID = PARENT_SKU_WITHVARIANTS,
                    CategoryID = CATEGORY_ID2,
                    SKUCategoryID = 2,
                    AllowAllOptions = true
                });

            Fake<SKUAllowedOptionInfo, SKUAllowedOptionInfoProvider>().WithData(
                new SKUAllowedOptionInfo
                {
                    SKUID = PARENT_SKU_WITHVARIANTS,
                    OptionSKUID = OPTION1_CAT1_ID
                },
                new SKUAllowedOptionInfo
                {
                    SKUID = PARENT_SKU_WITHVARIANTS,
                    OptionSKUID = OPTION2_CAT1_ID,

                },
                new SKUAllowedOptionInfo
                {
                    SKUID = PARENT_SKU_WITHVARIANTS,
                    OptionSKUID = OPTION1_CAT2_ID
                });

            Fake<VariantOptionInfo, VariantOptionInfoProvider>().WithData(
                new VariantOptionInfo
                {
                    VariantSKUID = VARIANT_SKU_ID1,
                    OptionSKUID = OPTION1_CAT1_ID,
                },
                new VariantOptionInfo
                {
                    VariantSKUID = VARIANT_SKU_ID1,
                    OptionSKUID = OPTION1_CAT2_ID,
                },
                new VariantOptionInfo
                {
                    VariantSKUID = VARIANT_SKU_ID2,
                    OptionSKUID = OPTION2_CAT1_ID,
                },
                new VariantOptionInfo
                {
                    VariantSKUID = VARIANT_SKU_ID2,
                    OptionSKUID = OPTION1_CAT2_ID,
                },
                new VariantOptionInfo
                {
                    VariantSKUID = VARIANT_SKU_ID3,
                    OptionSKUID = OPTION2_CAT1_ID,
                },
                new VariantOptionInfo
                {
                    VariantSKUID = VARIANT_SKU_ID3,
                    OptionSKUID = OPTION1_CAT2_ID,
                });
        }


        private void SetUpSKUs()
        {
            Fake<SKUInfo, SKUInfoProvider>().WithData(
                new SKUInfo
                {
                    SKUID = PARENT_SKU_WITHOUTVARIANTS,
                    SKUSiteID = SITE_ID1,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = PARENT_SKU_WITHVARIANTS,
                    SKUSiteID = SITE_ID1,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = VARIANT_SKU_ID1,
                    SKUSiteID = SITE_ID1,
                    SKUParentSKUID = PARENT_SKU_WITHVARIANTS,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = VARIANT_SKU_ID2,
                    SKUSiteID = SITE_ID1,
                    SKUParentSKUID = PARENT_SKU_WITHVARIANTS,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = VARIANT_SKU_ID3,
                    SKUSiteID = SITE_ID1,
                    SKUParentSKUID = PARENT_SKU_WITHVARIANTS,
                    SKUEnabled = false,
                    SKUCreated = DateTime.Now
                },
                new SKUInfo
                {
                    SKUID = VARIANT_CROSSSITE_ID,
                    SKUSiteID = SITE_ID2,
                    SKUParentSKUID = PARENT_SKU_WITHVARIANTS,
                    SKUEnabled = true,
                    SKUCreated = DateTime.Now
                });
        }
    }
}
