using CMS.Ecommerce;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    class ShoppingCartValidatorTests : EcommerceTestsBase
    {
        private SiteInfo mDifferentSite;
        private CustomerInfo mDifferentCustomer;


        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            SetUpSite();
            UserInfoProvider.AddUserToSite(Factory.DefaultUser.UserName, Factory.SiteInfo.SiteName);

            mDifferentSite = new SiteInfo()
            {
                DomainName = "1.2.3.4",
                SiteName = "DifferentSite",
                DisplayName = "DifferentSite",
                Status = SiteStatusEnum.Running,
            };
            mDifferentSite.Insert();

            mDifferentCustomer = new CustomerInfo()
            {
                CustomerFirstName = "Alfred",
                CustomerLastName = "Derfla",
                CustomerEmail = "em@a.il",
                CustomerSiteID = mDifferentSite.SiteID
            };
            mDifferentCustomer.Insert();
        }


        [SetUp]
        public void SetUp()
        {
            // Reset factory objects so it is not necessary to recreate them before each test case
            Factory.DefaultUser.Enabled = true;
            Factory.PaymentMethodDefault.PaymentOptionSiteID = Factory.SiteInfo.SiteID;
            Factory.ShippingOptionDefault.ShippingOptionEnabled = true;
            Factory.ShippingOptionDefault.ShippingOptionSiteID = Factory.SiteInfo.SiteID;
            Factory.CustomerAddressUSA.AddressCountryID = Factory.CountryUSA.CountryID;
            Factory.CustomerAddressUSA.AddressCustomerID = Factory.CustomerAnonymous.CustomerID;
            Factory.CustomerAddressCZE.AddressCountryID = Factory.CountryCZE.CountryID;
            Factory.CustomerAddressCZE.AddressCustomerID = Factory.CustomerAnonymous.CustomerID;
        }


        [Test]
        public void Validate_DisabledUser_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.User.Enabled = false;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.UserDisabled)
            );
        }


        [Test]
        public void Validate_PaymentMethodDisabled_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.PaymentMethod = Factory.PaymentMethodDisabled;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.PaymentMethodDisabled)
            );
        }


        [Test]
        public void Validate_PaymentMethodFromDifferentSite_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.PaymentMethod.PaymentOptionSiteID = mDifferentSite.SiteID;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.PaymentMethodFromDifferentSite)
            );
        }


        [Test]
        public void Validate_ShoppingOptionDisabled_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.ShippingOption.ShippingOptionEnabled = false;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.ShoppingOptionDisabled)
            );
        }


        [Test]
        public void Validate_ShippingOptionFromDifferentSite_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.ShippingOption.ShippingOptionSiteID = mDifferentSite.SiteID;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.ShippingOptionFromDifferentSite)
            );
        }


        [Test]
        public void Validate_InvalidBillingAddress_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.BillingAddress.Country = null;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.NotNull(validator.BillingAddress),
                () => Assert.IsTrue(validator.BillingAddress.CheckFailed)
            );
        }


        [Test]
        public void Validate_InvalidShippingAddress_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.ShippingAddress.Country = null;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.NotNull(validator.ShippingAddress),
                () => Assert.IsTrue(validator.ShippingAddress.CheckFailed)
            );
        }


        [Test]
        public void Validate_BillingAddressFromDifferentCustomer_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.BillingAddress.OriginalAddress.AddressCustomerID = mDifferentCustomer.CustomerID;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.BillingAddressFromDifferentCustomer)
            );
        }


        [Test]
        public void Validate_ShippingAddressFromDifferentCustomer_CheckFailed()
        {
            var cart = CreateValidShoppingCart();
            cart.ShippingAddress.OriginalAddress.AddressCustomerID = mDifferentCustomer.CustomerID;

            var validator = GetValidator(cart);

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.ShippingAddressFromDifferentCustomer)
            );
        }


        [Test]
        public void Validate_ValidCart_CheckPassed()
        {
            var cart = CreateValidShoppingCart();

            var validator = GetValidator(cart);

            Assert.IsFalse(validator.CheckFailed);
        }


        private ShoppingCart CreateValidShoppingCart()
        {
            var cartInfo = new ShoppingCartInfo
            {
                ShoppingCartSiteID = SiteInfo.SiteID,
                ShoppingCartCurrencyID = Factory.MainCurrency.CurrencyID,
                ShoppingCartPaymentOptionID = Factory.PaymentMethodDefault.PaymentOptionID,
                ShoppingCartShippingOptionID = Factory.ShippingOptionDefault.ShippingOptionID,
                ShoppingCartCustomerID = Factory.CustomerAnonymous.CustomerID,
                ShoppingCartBillingAddress = Factory.CustomerAddressUSA,
                ShoppingCartShippingAddress = Factory.CustomerAddressCZE
            };
            cartInfo.Insert();
            cartInfo.Evaluate();

            var cart = new ShoppingCart(cartInfo);
            cart.User = Factory.DefaultUser;

            return cart;
        }


        private ShoppingCartValidator GetValidator(ShoppingCart cart)
        {
            var validator = new ShoppingCartValidator(cart);
            validator.Validate();
            return validator;
        }
    }
}
