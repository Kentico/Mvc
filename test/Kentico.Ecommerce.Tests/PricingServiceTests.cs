using CMS.Ecommerce;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    class PricingServiceTests : EcommerceTestsBase
    {
        private readonly PricingService mPricingService = new PricingService();

        [OneTimeSetUp]
        public void SetUp()
        {
            SetUpSite();
            CreateDiscounts();
        }


        private ShoppingCart CreateEmptyShoppingCartWithBillingAddress()
        {
            return CreateCartWithCustomerInfo(null, Factory.CustomerAddressUSA);
        }


        private SKUInfo CreateSKUInfo(double skuPrice, double listPrice, string name)
        {
            var sku = new SKUInfo
            {
                SKUSiteID = SiteID,
                SKUPrice = skuPrice,
                SKURetailPrice = listPrice,
                SKUName = name + Factory.NewUniqueName(),
                SKUEnabled = true
            };

            SKUInfoProvider.SetSKUInfo(sku);

            return sku;
        }


        private void CreateDiscounts()
        {
            DiscountInfoProvider.SetDiscountInfo(new DiscountInfo
            {
                DiscountName = "FlatCatalogDiscount",
                DiscountDisplayName = "Flat Catalog Discount",
                DiscountSiteID = SiteID,
                DiscountOrder = 1,
                DiscountEnabled = true,
                DiscountUsesCoupons = false,
                DiscountApplyTo = DiscountApplicationEnum.Products,
                DiscountCustomerRestriction = DiscountCustomerEnum.All,
                DiscountApplyFurtherDiscounts = true,
                ItemDiscountIsFlat = true,
                DiscountValue = 10
            });

            DiscountInfoProvider.SetDiscountInfo(new DiscountInfo
            {
                DiscountName = "RelativeCatalogDiscount",
                DiscountDisplayName = "Relative Catalog Discount",
                DiscountSiteID = SiteID,
                DiscountOrder = 1,
                DiscountEnabled = true,
                DiscountUsesCoupons = false,
                DiscountApplyTo = DiscountApplicationEnum.Products,
                DiscountCustomerRestriction = DiscountCustomerEnum.All,
                DiscountApplyFurtherDiscounts = true,
                ItemDiscountIsFlat = false,
                DiscountValue = 10
            });

            DiscountInfoProvider.SetDiscountInfo(new DiscountInfo
            {
                DiscountName = "RelativeCatalogDiscount2",
                DiscountDisplayName = "Relative Catalog Discount 2",
                DiscountSiteID = SiteID,
                DiscountOrder = 2,
                DiscountEnabled = true,
                DiscountUsesCoupons = false,
                DiscountApplyTo = DiscountApplicationEnum.Products,
                DiscountCustomerRestriction = DiscountCustomerEnum.All,
                DiscountApplyFurtherDiscounts = true,
                ItemDiscountIsFlat = false,
                DiscountValue = 10
            });
        }


        [TestCase("SKUWithPriceAndRetailPrice", 99.0049, 100.0049, 99, 100, 0, 0)]
        [TestCase("SKUWithRetailPriceLowerThanPrice", 98.999, 50, 99, 50, 0, 0)]
        [TestCase("SKUWithZeroPrices", 0, 0, 0, 0, 0, 0)]
        [TestCase("SKUWithSamePriceAndRetailPrice", 100, 100, 100, 100, 0, 0)]
        public void PriceCalculatedWithoutTaxesDiscounts(string skuName, double skuPrice, double listPrice, decimal expectedPrice, decimal expectedListPrice, decimal expectedDiscount, decimal expectedtax)
        {
            var cart = CreateEmptyShoppingCartWithBillingAddress();
            var sku = CreateSKUInfo(skuPrice, listPrice, skuName);
            var calculatedPrice = mPricingService.CalculatePrice(sku, cart, false, false);

            var expectedResult = new ProductPrice
            {
                Discount = expectedDiscount,
                ListPrice = expectedListPrice,
                Price = expectedPrice,
                Tax = expectedtax
            };

            AssertPrice(calculatedPrice, expectedResult);
        }


        [TestCase("SKUWithPriceAndRetailPrice", 99.0049, 100, 71.19, 100, 27.81, 0)]
        [TestCase("SKUWithRetailPriceLowerThanPrice", 98.999, 50, 71.19, 50, 27.81, 0)]
        [TestCase("SKUWithZeroPrices", 0, 0, 0, 0, 0, 0)]
        [TestCase("SKUWithSamePriceAndRetailPrice", 100, 100, 72, 100, 28, 0)]
        public void PriceCalculatedWithDiscounts(string skuName, double skuPrice, double listPrice, decimal expectedPrice, decimal expectedListPrice, decimal expectedDiscount, decimal expectedtax)
        {
            var cart = CreateEmptyShoppingCartWithBillingAddress();
            var sku = CreateSKUInfo(skuPrice, listPrice, skuName);
            var calculatedPrice = mPricingService.CalculatePrice(sku, cart, true, false);

            var expectedResult = new ProductPrice
            {
                Discount = expectedDiscount,
                ListPrice = expectedListPrice,
                Price = expectedPrice,
                Tax = expectedtax
            };

            AssertPrice(calculatedPrice, expectedResult);
        }


        [TestCase("SKUWithPriceAndRetailPrice", 99.0049, 100, 85.43, 120, 27.81, 14.24)]
        [TestCase("SKUWithRetailPriceLowerThanPrice", 98.999, 50, 85.43, 60, 27.81, 14.24)]
        [TestCase("SKUWithZeroPrices", 0, 0, 0, 0, 0, 0)]
        [TestCase("SKUWithSamePriceAndRetailPrice", 100, 100, 86.4, 120, 28, 14.4)]
        public void PriceCalculatedWithDiscountsAndTaxes(string skuName, double skuPrice, double listPrice, decimal expectedPrice, decimal expectedListPrice, decimal expectedDiscount, decimal expectedtax)
        {
            var cart = CreateEmptyShoppingCartWithBillingAddress();
            var sku = CreateSKUInfo(skuPrice, listPrice, skuName);

            SKUTaxClassInfoProvider.AddTaxClassToSKU(Factory.TaxClassDefault.TaxClassID, sku.SKUID);

            var calculatedPrice = mPricingService.CalculatePrice(sku, cart, true, true);

            var expectedResult = new ProductPrice
            {
                Discount = expectedDiscount,
                ListPrice = expectedListPrice,
                Price = expectedPrice,
                Tax = expectedtax
            };

            AssertPrice(calculatedPrice, expectedResult);
        }
    }
}
