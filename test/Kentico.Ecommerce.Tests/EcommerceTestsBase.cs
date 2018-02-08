using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using Kentico.Ecommerce.Tests.Fakes;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    public class EcommerceTestsBase : IsolatedIntegrationTests
    {
        private readonly EcommerceFakeFactory mFakeFactory = new EcommerceFakeFactory();

        protected EcommerceFakeFactory Factory => mFakeFactory;

        protected SiteInfo SiteInfo => mFakeFactory.SiteInfo;

        protected int SiteID => mFakeFactory.SiteInfo.SiteID;


        protected void SetUpSite()
        {
            InsertDefaultSite();
            InsertDefaultMainCurrency();
            InsertDefaultGlobalMainCurrency();
            InsertDefaultUser();

            InsertDefaultOrderStatuses();            
            InsertDefaultCountries();
            InsertDefaultStates();
            InsertDefaultTaxClasses();            
            InsertDefaultCustomers();
            InsertDefaultPaymentMethods();
            InsertDefaultShippings();
            InsertDefaultSKUs();
        }


        protected void InsertDefaultSite()
        {
            SiteInfoProvider.SetSiteInfo(mFakeFactory.InitSite());
        }


        protected void InsertDefaultMainCurrency()
        {
            SettingsKeyInfoProvider.SetValue(ECommerceSettings.USE_GLOBAL_CURRENCIES, SiteID, false);
            mFakeFactory.InitMainCurrency(SiteID).Insert();
        }


        protected void InsertDefaultGlobalMainCurrency()
        {
            mFakeFactory.InitMainCurrency(null).Insert();
        }


        protected void InsertDefaultOrderStatuses()
        {
            SettingsKeyInfoProvider.SetValue(ECommerceSettings.USE_GLOBAL_ORDER_STATUS, SiteID, false);
            mFakeFactory.InitOrderStatuses().InsertDB();
        }


        protected void InsertDefaultUser()
        {
            mFakeFactory.InitUser().Insert();
        }


        protected void InsertDefaultCountries()
        {
            mFakeFactory.InitCountries().InsertDB();
        }


        protected void InsertDefaultStates()
        {
            mFakeFactory.InitStates().InsertDB();
        }


        protected void InsertDefaultTaxClasses()
        {
            mFakeFactory.InitTaxClasses().InsertDB();
            mFakeFactory.InitTaxClassCountries().InsertDB();
            mFakeFactory.InitTaxClassStates().InsertDB();
        }


        protected void InsertDefaultCustomers()
        {
            mFakeFactory.InitCustomers().InsertDB();
            mFakeFactory.InitCustomerAddresses().InsertDB();
        }


        protected void InsertDefaultShippings()
        {
            mFakeFactory.InitCarriers().InsertDB();
            mFakeFactory.InitShippingOptions().InsertDB();
            mFakeFactory.InitShippingCosts().InsertDB();
        }
        

        protected void InsertDefaultPaymentMethods()
        {
            mFakeFactory.InitPaymentMethods().InsertDB();
        }


        public void InsertDefaultSKUs()
        {
            mFakeFactory.InitSKUs().InsertDB();

            mFakeFactory.SKUWithTaxes.SKUTaxClassID = mFakeFactory.TaxClassDefault.TaxClassID;

            mFakeFactory.SKUWithTaxes.Update();
        }

        
        public ShoppingCart CreateCartWithItem(int skuId, int units = 1)
        {
            var cart = CreateEmptyShoppingCart();

            cart.AddItem(skuId, units);

            return cart;
        }


        public ShoppingCart CreateCartWithCustomerInfo(CustomerInfo customer, AddressInfo address = null, ICurrentContactProvider currentContactProvider = null, IContactProcessingChecker contactProcessingChecker = null)
        {
            var cartInfo = ShoppingCartFactory.CreateCart(SiteID);
            cartInfo.ShoppingCartCurrencyID = Factory.MainCurrency.CurrencyID;
            cartInfo.Customer = customer;
            cartInfo.ShoppingCartBillingAddress = address;
            cartInfo.Evaluate();

            ShoppingCartInfoProvider.SetShoppingCartInfo(cartInfo);

            return new ShoppingCart(cartInfo, new EcommerceActivitiesLoggerFake(), currentContactProvider, contactProcessingChecker);
        }
        

        public ShoppingCart CreateEmptyShoppingCart()
        {
            var originalCart = ShoppingCartFactory.CreateCart(SiteID);

            ShoppingCartInfoProvider.SetShoppingCartInfo(originalCart);

            return new ShoppingCart(originalCart, new EcommerceActivitiesLoggerFake(), null, null);
        }


        protected void AssertPrice(ProductPrice calculatedprice, ProductPrice expectedPrice)
        {
            CMSAssert.All(
                () => Assert.AreEqual(expectedPrice.Discount, calculatedprice.Discount, "Discount {0} does not match the expected value {1}.", new object[] { calculatedprice.Discount, expectedPrice.Discount }),
                () => Assert.AreEqual(expectedPrice.ListPrice, calculatedprice.ListPrice, "ListPrice {0} does not match the expected value {1}.", new object[] { calculatedprice.ListPrice, expectedPrice.ListPrice }),
                () => Assert.AreEqual(expectedPrice.Price, calculatedprice.Price, "Price {0} does not match the expected value {1}.", new object[] { calculatedprice.Price, expectedPrice.Price }),
                () => Assert.AreEqual(expectedPrice.Tax, calculatedprice.Tax, "Tax {0} does not match the expected value {1}.", new object[] { calculatedprice.Tax, expectedPrice.Tax })
            );
        }
    }
}
