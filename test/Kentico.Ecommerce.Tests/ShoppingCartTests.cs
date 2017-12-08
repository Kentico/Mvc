using System;
using System.Linq;

using CMS.Activities;
using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using Kentico.Ecommerce.Tests.Fakes;

using NSubstitute;
using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    public class ShoppingCartTests : EcommerceTestsBase
    {
        private string ORDER_COUPON_CODE = "OrderCouponCode";
        private string GIFT_CARD_COUPON_CODE = "GiftCardCouponCode";


        [OneTimeSetUp]
        public void SetUp()
        {
            SetUpSite();
            CreateOrderDiscounts();
            CreateGiftCard();
        }


        [SetUp]
        public void TestSetUp()
        {
            Service.Use<IActivityLogService, ActivityLogServiceFake>(Guid.NewGuid().ToString());
            Service.Use<ILocalizationService, LocalizationServiceFake>(Guid.NewGuid().ToString());
        }


        private ContactInfo CreateContact()
        {
            var contact = new ContactInfo
            {
                ContactLastName = "contactlastname" + Guid.NewGuid(),
            };
            ContactInfoProvider.SetContactInfo(contact);
            return contact;
        }


        private void CreateOrderDiscounts()
        {
            DiscountInfo orderDiscount = Factory.NewOrderDiscount(10);
            orderDiscount.DiscountUsesCoupons = true;
            orderDiscount.Insert();

            new CouponCodeInfo
            {
                CouponCodeDiscountID = orderDiscount.DiscountID,
                CouponCodeCode = ORDER_COUPON_CODE,
            }.Insert();
        }


        private void CreateGiftCard()
        {
            GiftCardInfo giftCard = Factory.NewGiftCard(1);
            giftCard.Insert();

            new GiftCardCouponCodeInfo
            {
                GiftCardCouponCodeGiftCardID = giftCard.GiftCardID,
                GiftCardCouponCodeCode = GIFT_CARD_COUPON_CODE
            }.Insert();
        }


        private ShoppingCart CreateCartWithMultipleValidItems()
        {
            var cart = CreateEmptyShoppingCart();

            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.AddItem(Factory.SKULimited.SKUID);

            return cart;
        }


        [TestCase(1)]
        [TestCase(10)]
        public void AddItem_ContainsCorrectItem(int units)
        {
            var cart = CreateCartWithItem(Factory.SKUAvailable.SKUID, units);
            var item = cart.Items.FirstOrDefault();

            CMSAssert.All(
                () => Assert.That(cart.Items.Count() == 1, "Wrong number of products in shopping cart", null),
                () => Assert.AreEqual(item.Name, Factory.SKUAvailable.SKUName, "Item name does not match", null),
                () => Assert.AreEqual(item.Units, units, "Invalid number of item units", null)
            );
        }


        [Test]
        public void AddItem_ContainsMultipleItems()
        {
            var cart = CreateEmptyShoppingCart();

            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.AddItem(Factory.SKULimited.SKUID);

            Assert.AreEqual(cart.Items.Count(), 2);
        }


        [Test]
        public void AddItem_ContainsNoItem()
        {
            var cart = CreateEmptyShoppingCart();

            cart.AddItem(-8, 1);
            cart.AddItem(Factory.SKUAvailable.SKUID, 0);
            cart.AddItem(Factory.SKUAvailable.SKUID, -1);

            Assert.That(cart.IsEmpty);
        }


        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(10, 10)]
        public void UpdateUnits_ItemIsUpdated(int units, int updatedUnits)
        {
            var cart = CreateCartWithItem(Factory.SKUAvailable.SKUID, units);
            var item = cart.Items.FirstOrDefault();

            cart.UpdateQuantity(item.ID, updatedUnits);

            Assert.AreEqual(item.Units, updatedUnits);
        }


        [Test]
        public void RemoveItem_ItemIsRemoved()
        {
            var cart = CreateCartWithItem(Factory.SKUAvailable.SKUID);
            var item = cart.Items.FirstOrDefault();

            cart.RemoveItem(item.ID);

            Assert.That(cart.IsEmpty);
        }


        [Test]
        public void RemoveAllItems_CartIsEmpty()
        {
            var cart = CreateCartWithMultipleValidItems();
            cart.RemoveAllItems();

            Assert.That(cart.IsEmpty);
        }


        [Test]
        public void Validate_CartIsValid()
        {
            var cart = CreateCartWithMultipleValidItems();
            var checkResult = cart.Validate();

            Assert.That(!checkResult.CheckFailed);
        }


        [Test]
        public void ValidateCartWithDisabledProduct_CartIsNotValid()
        {
            var cart = CreateCartWithItem(Factory.SKUDisabled.SKUID);
            var checkResult = cart.ValidateContent();

            Assert.That(checkResult.CheckFailed);
        }


        [Test]
        public void ValidateCartWithLimitedProduct_CartIsNotValid()
        {
            var cart = CreateCartWithItem(Factory.SKULimited.SKUID, 15);
            var checkResult = cart.ValidateContent();

            Assert.That(checkResult.CheckFailed);
        }


        [TestCase(true)]
        [TestCase(false)]
        public void ShoppingCart_IsShippingNeeded(bool isShippingNeeded)
        {
            var sku = Factory.NewSKU();
            sku.SKUNeedsShipping = isShippingNeeded;
            SKUInfoProvider.SetSKUInfo(sku);

            var cart = CreateCartWithItem(sku.SKUID);

            Assert.That(cart.IsShippingNeeded == isShippingNeeded);
        }


        [Test]
        public void ShoppingCart_ShippingIsSet()
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID);

            cart.ShippingOption = Factory.ShippingOptionDefault;
            cart.OriginalCart.Evaluate();


            Assert.That(Factory.ShippingOptionDefault.ShippingOptionID == cart.ShippingOption.ShippingOptionID, "Shipping option is not set");
        }


        [Test]
        public void ShoppingCart_PaymentIsSet()
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.PaymentMethod = Factory.PaymentMethodDefault;
            cart.OriginalCart.Evaluate();

            Assert.That(Factory.PaymentMethodDefault.PaymentOptionID == cart.PaymentMethod.PaymentOptionID, "Payment option is not set");
        }


        [Test]
        public void ShoppingCart_BillingAddressIsSet()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);

            cart.BillingAddress = new CustomerAddress(Factory.CustomerAddressUSA);
            cart.Save();

            var expectedBillingAddress = cart.BillingAddress;
            var originalCartAddress = cart.OriginalCart.ShoppingCartBillingAddress;

            CMSAssert.All(
                () => Assert.That(originalCartAddress.AddressID > 0, "Billing address is not set"),
                () => Assert.AreEqual(expectedBillingAddress.CountryID, originalCartAddress.AddressCountryID, "Country ID is different."),
                () => Assert.AreEqual(expectedBillingAddress.StateID, originalCartAddress.AddressStateID, "State ID is different."),
                () => Assert.AreEqual(expectedBillingAddress.Line1, originalCartAddress.AddressLine1, "Line 1 is different."),
                () => Assert.AreEqual(expectedBillingAddress.Line2, originalCartAddress.AddressLine2, "Line 2 is different."),
                () => Assert.AreEqual(expectedBillingAddress.PostalCode, originalCartAddress.AddressZip, "Postal code is different."),
                () => Assert.AreEqual(expectedBillingAddress.PersonalName, originalCartAddress.AddressPersonalName, "Personal name is different.")
            );
        }


        [Test]
        public void ShoppingCart_ShippingAddressIsSet()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);

            cart.ShippingAddress = new CustomerAddress(Factory.CustomerAddressUSA);
            cart.Save();

            var expectedShippingAddress = cart.ShippingAddress;
            var originalCartAddress = cart.OriginalCart.ShoppingCartShippingAddress;

            CMSAssert.All(
                () => Assert.That(originalCartAddress.AddressID > 0, "Shipping address is not set"),
                () => Assert.AreEqual(expectedShippingAddress.CountryID, originalCartAddress.AddressCountryID, "Country ID is different."),
                () => Assert.AreEqual(expectedShippingAddress.StateID, originalCartAddress.AddressStateID, "State ID is different."),
                () => Assert.AreEqual(expectedShippingAddress.Line1, originalCartAddress.AddressLine1, "Line 1 is different."),
                () => Assert.AreEqual(expectedShippingAddress.Line2, originalCartAddress.AddressLine2, "Line 2 is different."),
                () => Assert.AreEqual(expectedShippingAddress.PostalCode, originalCartAddress.AddressZip, "Postal code is different."),
                () => Assert.AreEqual(expectedShippingAddress.PersonalName, originalCartAddress.AddressPersonalName, "Personal name is different.")
            );
        }


        [Test]
        public void ShoppingCart_BillingAddressIsUpdated()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.OriginalCart.ShoppingCartBillingAddress = Factory.CustomerAddressUSA;

            var expectedLine1 = "New line1";
            var expectedLine2 = "New line2";
            var expectedPersonalName = "New personal Name";
            var expectedPostalCode = "New ZIP";

            // Modify address
            var address = cart.BillingAddress;
            address.Line1 = expectedLine1;
            address.Line2 = expectedLine2;
            address.PersonalName = expectedPersonalName;
            address.PostalCode = expectedPostalCode;

            var originalCartAddress = cart.OriginalCart.ShoppingCartBillingAddress;

            CMSAssert.All(
                () => Assert.That(originalCartAddress.AddressID == Factory.CustomerAddressUSA.AddressID, "A different billing address was assigned to the site."),
                () => Assert.AreEqual(expectedLine1, originalCartAddress.AddressLine1, "Line 1 is different."),
                () => Assert.AreEqual(expectedLine2, originalCartAddress.AddressLine2, "Line 2 is different."),
                () => Assert.AreEqual(expectedPostalCode, originalCartAddress.AddressZip, "Postal code is different."),
                () => Assert.AreEqual(expectedPersonalName, originalCartAddress.AddressPersonalName, "Personal name is different.")
            );
        }


        [Test]
        public void ShoppingCart_ShippingAddressIsUpdated()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.OriginalCart.ShoppingCartShippingAddress = Factory.CustomerAddressUSA;

            var expectedLine1 = "New line1";
            var expectedLine2 = "New line2";
            var expectedPersonalName = "New personal Name";
            var expectedPostalCode = "New ZIP";

            // Modify address
            var address = cart.ShippingAddress;
            address.Line1 = expectedLine1;
            address.Line2 = expectedLine2;
            address.PersonalName = expectedPersonalName;
            address.PostalCode = expectedPostalCode;

            var originalCartAddress = cart.OriginalCart.ShoppingCartShippingAddress;

            CMSAssert.All(
                () => Assert.That(originalCartAddress.AddressID == Factory.CustomerAddressUSA.AddressID, "A different shipping address was assigned to the site."),
                () => Assert.AreEqual(expectedLine1, originalCartAddress.AddressLine1, "Line 1 is different."),
                () => Assert.AreEqual(expectedLine2, originalCartAddress.AddressLine2, "Line 2 is different."),
                () => Assert.AreEqual(expectedPostalCode, originalCartAddress.AddressZip, "Postal code is different."),
                () => Assert.AreEqual(expectedPersonalName, originalCartAddress.AddressPersonalName, "Personal name is different.")
            );
        }


        [TestCase(1)]
        [TestCase(5)]
        public void ShoppingCart_ItemPriceIsCorrect(int units)
        {
            var cart = CreateCartWithItem(Factory.SKUAvailable.SKUID, units);

            var item = cart.Items.FirstOrDefault();
            var originalItem = item.OriginalCartItem;

            CMSAssert.All(
                () => Assert.AreEqual(item.Subtotal, originalItem.TotalPrice),
                () => Assert.AreEqual(item.UnitPrice, originalItem.UnitPrice)
            );
        }


        [Test]
        public void ShoppingCart_TotalPriceIsCorrect()
        {
            var cart = CreateCartWithMultipleValidItems();
            var originalCart = cart.OriginalCart;

            Assert.AreEqual(cart.TotalPrice, originalCart.TotalPrice);
        }


        [Test]
        public void ShoppingCart_TaxIsCorrect()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);

            var billingAddress = new CustomerAddress(Factory.CustomerAddressUSA);
            cart.BillingAddress = billingAddress;

            var originalCart = cart.OriginalCart;

            Assert.AreEqual(cart.TotalTax, originalCart.TotalTax);
        }


        [Test]
        public void ShoppingCart_TaxIsChanged()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);

            cart.BillingAddress = new CustomerAddress(Factory.CustomerAddressUSA);

            cart.RemoveAllItems();
            cart.AddItem(Factory.SKUAvailable.SKUID);

            Assert.AreEqual(cart.TotalTax, 0);
        }


        [Test]
        public void ShoppingCart_OrderDiscountCouponApplied()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);
            cart.AddCouponCode(ORDER_COUPON_CODE);
            //cart.Evaluate();

            CMSAssert.All(
                () => Assert.AreEqual(Factory.SKUWithTaxes.SKUPrice, cart.Items.Sum(item => item.Subtotal), "Subtotal is not calculated properly."),
                () => Assert.Greater(Factory.SKUWithTaxes.SKUPrice, cart.TotalPrice, "Order discount is not applied with a valid code."),
                () => Assert.Greater(Factory.SKUWithTaxes.SKUPrice, cart.GrandTotal, "Order discount is not applied with a valid code."),
                () => Assert.IsTrue(cart.AppliedCouponCodes.Any(), "Order discount coupon is not applied."),
                () => Assert.That(cart.AppliedCouponCodes, Is.Not.Null.Or.Empty, "Order discount coupon is not assigned.")
            );
        }


        [Test]
        public void ShoppingCart_InvalidOrderDiscountCoupon()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);
            cart.AddCouponCode("INVALIDCODE");
            //cart.Evaluate();

            CMSAssert.All(
                () => Assert.AreEqual(Factory.SKUWithTaxes.SKUPrice, cart.GrandTotal, "Order discount is applied with an invalid code."),
                () => Assert.IsFalse(cart.AppliedCouponCodes.Any(), "Invalid order discount coupon is applied."),
                () => Assert.That(cart.AppliedCouponCodes, Is.Not.Null.Or.Empty, "Order discount coupon is not assigned.")
            );
        }


        [Test]
        public void ShoppingCart_GiftCardCouponApplied()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);
            cart.AddCouponCode(GIFT_CARD_COUPON_CODE);
            //cart.Evaluate();

            CMSAssert.All(
                () => Assert.AreEqual(Factory.SKUWithTaxes.SKUPrice, cart.Items.Sum(item => item.Subtotal), "Subtotal is not calculated properly."),
                () => Assert.Greater(Factory.SKUWithTaxes.SKUPrice, cart.GrandTotal, "Gift card is not applied with a valid code."),
                () => Assert.IsTrue(cart.AppliedCouponCodes.Any(), "Gift card coupon is not applied."),
                () => Assert.That(cart.AppliedCouponCodes, Is.Not.Null.Or.Empty, "Gift card coupon is not assigned.")
            );
        }


        [Test]
        public void ShoppingCart_InvalidGiftCardCoupon()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);
            cart.AddCouponCode("INVALIDCODE");
            //cart.Evaluate();

            CMSAssert.All(
                () => Assert.AreEqual(Factory.SKUWithTaxes.SKUPrice, cart.GrandTotal, "Gift card is applied with an invalid code."),
                () => Assert.IsFalse(cart.AppliedCouponCodes.Any(), "Invalid gift card coupon is applied."),
                () => Assert.That(cart.AppliedCouponCodes, Is.Not.Null.Or.Empty, "Gift card coupon is not assigned.")
            );
        }


        [Test]
        public void ShoppingCart_ApplyMultipleCoupons()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);
            cart.AddCouponCode(ORDER_COUPON_CODE);
            cart.AddCouponCode(GIFT_CARD_COUPON_CODE);

            CMSAssert.All(
                () => Assert.Greater(Factory.SKUWithTaxes.SKUPrice, cart.GrandTotal, "Grand total is not calculated properly."),
                () => Assert.AreEqual(2, cart.AppliedCouponCodes.Count(), "Some coupons are not applied."),
                () => Assert.That(cart.AppliedCouponCodes, Is.Not.Null.Or.Empty, "Coupons are not applied.")
            );
        }


        [Test]
        public void ShoppingCart_AddAndRemoveMultipleCoupons()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.AddItem(Factory.SKUWithTaxes.SKUID);
            cart.AddCouponCode(ORDER_COUPON_CODE);
            cart.AddCouponCode(GIFT_CARD_COUPON_CODE);
            cart.RemoveCouponCode(ORDER_COUPON_CODE);
            cart.RemoveCouponCode(GIFT_CARD_COUPON_CODE);

            CMSAssert.All(
                () => Assert.AreEqual(Factory.SKUWithTaxes.SKUPrice, cart.GrandTotal, "Removed coupon is applied."),
                () => Assert.IsFalse(cart.AppliedCouponCodes.Any(), "Removed coupons are applied.")
            );
        }


        [Test]
        public void GetCustomer_UserIsLoggedIn_CustomerDetailsAreFilled()
        {
            var cart = CreateEmptyShoppingCart();
            cart.User = Factory.DefaultUser;

            var customer = cart.Customer;

            CMSAssert.All(
                () => Assert.That(customer.Email == Factory.DefaultUser.Email, "Email is different"),
                () => Assert.That(customer.FirstName == Factory.DefaultUser.FirstName, "First name is different"),
                () => Assert.That(customer.LastName == Factory.DefaultUser.LastName, "Last name is different")
            );
        }


        [Test]
        public void AddItem_LogsActivity()
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID);

            var activityLogger = (ActivityLogServiceFake)Service.Resolve<IActivityLogService>();
            var productInShoppingCart = activityLogger.LoggedActivities.First();

            CMSAssert.All(
                () => Assert.That(productInShoppingCart != null, "Product added to shopping cart activity is not logged"),
                () => Assert.That(productInShoppingCart.ActivityItemID, Is.EqualTo(Factory.SKUAvailable.SKUID), "SkuID differs"),
                () => Assert.That(productInShoppingCart.ActivityTitle, Is.EqualTo(Factory.SKUAvailable.SKUName), "Sku name differs"),
                () => Assert.That(productInShoppingCart.ActivityValue, Is.EqualTo("1"), "Quantity differs")
                );
        }


        [Test]
        public void CartSave_UpdateContactRelations_ContactIsUpdated()
        {
            SiteContext.CurrentSite = SiteInfo;
            var contact = CreateContact();
            var customer = Factory.CustomerAnonymous;

            var currentContactProvider = Substitute.For<ICurrentContactProvider>();
            currentContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()).Returns(contact);

            var cart = CreateCartWithCustomerInfo(customer, null, currentContactProvider);

            cart.Save();

            CMSAssert.All(
                () => Assert.That(contact.ContactFirstName, Is.EqualTo(customer.CustomerFirstName), "Contact first name has not been updated"),
                () => Assert.That(contact.ContactLastName, Is.EqualTo(contact.ContactLastName), "Contact last name has been update (by default it should be unchanged)"),
                () => Assert.That(contact.ContactEmail, Is.EqualTo(customer.CustomerEmail), "Contact email has not been updated")
                );
        }
    }
}
