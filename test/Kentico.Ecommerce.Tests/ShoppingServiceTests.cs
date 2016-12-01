using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Activities;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Tests;
using CMS.SiteProvider;

using Kentico.Ecommerce.Tests.Fakes;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    public class ShoppingServiceTests : EcommerceTestsBase
    {
        private DiscountInfo ShippingDiscount;
        private ShoppingService ShoppingService;


        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            SetUpSite();
            CreateShippingDiscounts();
        }


        [SetUp]
        public void TestSetUp()
        {
            ObjectFactory<IActivityLogService>.SetObjectTypeTo<ActivityLogServiceFake>(true);
            ObjectFactory<ILocalizationService>.SetObjectTypeTo<LocalizationServiceFake>();
            OrderInfoProvider.ProviderObject = new OrderInfoProviderWithMailLogging();

            SiteContext.CurrentSite = SiteInfo;

            ShoppingService = new ShoppingService(new EcommerceActivitiesLoggerFake());

            // Setting order generates invoice which has a macro in the subject. The macro cannot be resolved and an error is logged.
            TestsEventLogProvider.FailForErrorEvent = false;
        }


        [Test]
        public void CreateOrder_NoCustomer_ThrowsException()
        {
            var cart = CreateEmptyShoppingCart();

            Assert.That(() => ShoppingService.CreateOrder(cart), Throws.Exception.TypeOf<InvalidOperationException>());
        }


        [Test]
        public void EmptyCart_CreateOrder_CreatesOrder()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);

            var order = ShoppingService.CreateOrder(cart);
            var orderInfo = OrderInfoProvider.GetOrderInfo(order.OrderID);

            CMSAssert.All(
                () => Assert.That(order.OrderID > 0, "Order does not have ID set."),
                () => Assert.That(orderInfo != null, "Order info not found in DB")
            );
        }


        [Test]
        public void NewCustomer_SetCustomer_SavesCustomer()
        {
            var newCustomer = new Customer
            {
                FirstName = "First",
                LastName = "Last",
                Email = "customer@test.test",
                PhoneNumber = "00 00 3.14",
                Company = "My Company",
                OrganizationID = "OrgID",
                TaxRegistrationID = "TaxID"
            };

            var cart = CreateEmptyShoppingCart();
            cart.Customer = newCustomer;
            cart.Save();

            var savedCustomer = cart.Customer;
            CMSAssert.All(
                () => Assert.NotNull(savedCustomer, "Saved customer is null"),
                () => Assert.That(savedCustomer.ID > 0, "Customer does not have ID set."),
                () => Assert.AreEqual(savedCustomer.ID, newCustomer.ID, "New customer does not have ID set.")
            );
        }


        [Test]
        public void EmptyCart_CreateOrder_SetsCustomer()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            var order = ShoppingService.CreateOrder(cart);
            var orderInfo = OrderInfoProvider.GetOrderInfo(order.OrderID);

            Assert.AreEqual(Factory.CustomerAnonymous.CustomerID, orderInfo.OrderCustomerID);
        }


        [Test]
        public void EmptyCart_CreateOrder_SetsAddresses()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous);
            cart.BillingAddress = new CustomerAddress(Factory.CustomerAddressCZE);
            cart.ShippingAddress = new CustomerAddress(Factory.CustomerAddressUSA);
            cart.ShippingOption = Factory.ShippingOptionDefault;

            var order = ShoppingService.CreateOrder(cart);
            var orderInfo = OrderInfoProvider.GetOrderInfo(order.OrderID);

            CMSAssert.All(
                () => Assert.That(orderInfo.OrderShippingAddress.AddressID > 0, "Order shipping address not stored in DB"),
                () => Assert.That(orderInfo.OrderBillingAddress.AddressID > 0, "Order billing address not stored in DB"),
                () => Assert.AreEqual(cart.ShippingAddress.PersonalName, orderInfo.OrderShippingAddress.AddressPersonalName, "Shipping addresses do not match"),
                () => Assert.AreEqual(cart.BillingAddress.PersonalName, orderInfo.OrderBillingAddress.AddressPersonalName, "Billing addresses do not match")
            );
        }


        [Test]
        public void CartWithShipping_CreateOrder_SetsShippingOption()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.ShippingOption = Factory.ShippingOptionDefault;

            var order = ShoppingService.CreateOrder(cart);
            var orderInfo = OrderInfoProvider.GetOrderInfo(order.OrderID);

            CMSAssert.All(
                () => Assert.That(orderInfo.OrderShippingOptionID > 0, "Order shipping option not stored in DB"),
                () => Assert.AreEqual(Factory.ShippingOptionDefault.ShippingOptionID, orderInfo.OrderShippingOptionID, "Shipping options do not match")
            );
        }


        [Test]
        public void CartWithoutShipping_CreateOrder_SetsNoShippingOption()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);

            var order = ShoppingService.CreateOrder(cart);
            var orderInfo = OrderInfoProvider.GetOrderInfo(order.OrderID);

            Assert.That(orderInfo.OrderShippingOptionID == 0, "Order shipping option is set");
        }


        [TestCase(1)]
        [TestCase(10)]
        [TestCase(42)]
        public void CartWithOneItem_CreateOrder_OrderItemIsSet(int unitsOrdered)
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.AddItem(Factory.SKUAvailable.SKUID, unitsOrdered);

            var order = ShoppingService.CreateOrder(cart);
            var orderItems = OrderItemInfoProvider.GetOrderItems(order.OrderID).ToList();

            Assert.AreEqual(1, orderItems.Count, "Wrong number of order items created");
            Assert.AreEqual(Factory.SKUAvailable.SKUID, orderItems[0].OrderItemSKUID, "Wrong product ordered");
            Assert.AreEqual(unitsOrdered, orderItems[0].OrderItemUnitCount, "Wrong number of products ordered");
        }


        [TestCase(1, 4)]
        [TestCase(2, 100)]
        [TestCase(100, 1000)]
        public void CartWithItems_CreateOrder_OrderItemsAreSet(int firstProductUnits, int secondProductUnits)
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.AddItem(Factory.SKUAvailable.SKUID, firstProductUnits);
            cart.AddItem(Factory.SKUWithoutShipping.SKUID, secondProductUnits);

            var order = ShoppingService.CreateOrder(cart);
            var orderItemsBySKUID = OrderItemInfoProvider.GetOrderItems(order.OrderID).ToDictionary(i => i.OrderItemSKUID);

            CMSAssert.All(
                () => Assert.AreEqual(2, orderItemsBySKUID.Count, "Wrong number of order items created"),
                () => Assert.AreEqual(firstProductUnits, orderItemsBySKUID[Factory.SKUAvailable.SKUID].OrderItemUnitCount, "Wrong number of SKUAvailable ordered"),
                () => Assert.AreEqual(secondProductUnits, orderItemsBySKUID[Factory.SKUWithoutShipping.SKUID].OrderItemUnitCount, "Wrong number of SKUWithoutShipping ordered")
            );
        }


        [Test]
        public void CartWithItems_CreateOrder_LoggsPurchaseActivity()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.AddItem(Factory.SKUWithoutShipping.SKUID);


            var order = ShoppingService.CreateOrder(cart);
            var mainCurrency = new Currency(CurrencyInfoProvider.GetMainCurrency(SiteContext.CurrentSiteID));
            var orderTotalPrice = (decimal)order.OriginalOrder.OrderTotalPriceInMainCurrency;

            var activityLogger = (ActivityLogServiceFake)Service.Entry<IActivityLogService>();
            var purchaseActivity = activityLogger.LoggedActivities.First(x => x.ActivityType == PredefinedActivityType.PURCHASE);
            var formattedPrice = mainCurrency.FormatPrice(orderTotalPrice);

            CMSAssert.All(
                () => Assert.That(purchaseActivity.ActivityItemID, Is.EqualTo(order.OrderID), "Order id differs"),
                () => Assert.That(purchaseActivity.ActivityValue, Is.EqualTo(orderTotalPrice.ToString(CultureHelper.EnglishCulture)), "Total price differs"),
                () => Assert.That(purchaseActivity.ActivityTitle, Is.EqualTo(formattedPrice), "Total price string differs")
                );
        }


        [Test]
        public void CartWithItems_CreateOrder_LoggsProductPurchasedActivities()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.AddItem(Factory.SKUWithoutShipping.SKUID, 10);

            ShoppingService.CreateOrder(cart);

            var activityLogger = (ActivityLogServiceFake)Service.Entry<IActivityLogService>();
            var productPurchasedActivity = activityLogger.LoggedActivities.Where(act => act.ActivityType == PredefinedActivityType.PURCHASEDPRODUCT).ToArray();
            var listItems = cart.Items.ToList();

            CMSAssert.All(
                () => Assert.That(productPurchasedActivity.Count, Is.EqualTo(2), "Logged different count of product purchased activities"),
                () => Assert.That(productPurchasedActivity[0].ActivityItemID, Is.EqualTo(listItems[0].OriginalCartItem.SKUID), "SKU ID of first product differs"),
                () => Assert.That(productPurchasedActivity[0].ActivityTitle, Is.EqualTo(listItems[0].Name), "SKU name of first product differs"),
                () => Assert.That(productPurchasedActivity[0].ActivityValue, Is.EqualTo(listItems[0].Units.ToString()), "Product quantity of first product differs"),
                () => Assert.That(productPurchasedActivity[1].ActivityItemID, Is.EqualTo(listItems[1].OriginalCartItem.SKUID), "SKU ID of second product differs"),
                () => Assert.That(productPurchasedActivity[1].ActivityTitle, Is.EqualTo(listItems[1].Name), "SKU name of second product differs"),
                () => Assert.That(productPurchasedActivity[1].ActivityValue, Is.EqualTo(listItems[1].Units.ToString()), "Product quantity of second product differs")
            );
        }


        [Test]
        public void SendingEmailsEnabled_CreateOrder_SendsNotifications()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.AddItem(Factory.SKUAvailable.SKUID);

            SettingsKeyInfoProvider.SetGlobalValue(ECommerceSettings.SEND_ORDER_NOTIFICATION, true);

            using (new CMSActionContext { SendEmails = true })
            {
                ShoppingService.CreateOrder(cart);
            }

            var logger = (OrderInfoProviderWithMailLogging)OrderInfoProvider.ProviderObject;

            CMSAssert.All(
                () => Assert.That(logger.EmailTemplatesSendToCustomer.Contains("Ecommerce.OrderNotificationToCustomer"), "'Thank you for order' notification not sent"),
                () => Assert.That(logger.EmailTemplatesSendToAdmin.Contains("Ecommerce.OrderNotificationToAdmin"), "'New order' notification not sent")
            );
        }


        [Test]
        public void SendingEmailsEnabled_CreateOrder_SendsStatusNotifications()
        {
            var cart = CreateCartWithCustomerInfo(Factory.CustomerAnonymous, Factory.CustomerAddressUSA);
            cart.AddItem(Factory.SKUAvailable.SKUID);

            SettingsKeyInfoProvider.SetGlobalValue(ECommerceSettings.SEND_ORDER_NOTIFICATION, true);

            using (new CMSActionContext { SendEmails = true })
            {
                ShoppingService.CreateOrder(cart);
            }

            var logger = (OrderInfoProviderWithMailLogging)OrderInfoProvider.ProviderObject;

            Assert.That(logger.EmailTemplatesSendToCustomer.Contains("Ecommerce.OrderStatusNotificationToCustomer"), "'Order status changed' notification not sent to customer");
            Assert.That(logger.EmailTemplatesSendToAdmin.Contains("Ecommerce.OrderStatusNotificationToAdmin"), "'Order status changed' notification not sent to admin");
        }


        [Test]
        public void BothAddressesSet_CreateOrder_AddressesAreDifferent()
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.Customer = new Customer(Factory.CustomerAnonymous);
            cart.BillingAddress = new CustomerAddress(Factory.CustomerAddressCZE);
            cart.ShippingAddress = new CustomerAddress(Factory.CustomerAddressUSA);

            var order = ShoppingService.CreateOrder(cart);

            var billingAddress = order.OriginalOrder.OrderBillingAddress;
            var shippingAddress = order.OriginalOrder.OrderShippingAddress;

            Assert.AreNotEqual(billingAddress.AddressPersonalName, shippingAddress.AddressPersonalName);
        }


        [Test]
        public void ShippingAddressNotSet_CreateOrder_AddressesAreEqual()
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.Customer = new Customer(Factory.CustomerAnonymous);
            cart.BillingAddress = new CustomerAddress(Factory.CustomerAddressCZE);

            var order = ShoppingService.CreateOrder(cart);

            var billingAddress = order.OriginalOrder.OrderBillingAddress;
            var shippingAddress = order.OriginalOrder.OrderShippingAddress;

            Assert.AreEqual(billingAddress.AddressPersonalName, shippingAddress.AddressPersonalName);
        }


        [Test]
        public void ShippingAddressIsNull_CreateOrder_AddressesAreEqual()
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID);
            cart.Customer = new Customer(Factory.CustomerAnonymous);
            cart.BillingAddress = new CustomerAddress(Factory.CustomerAddressCZE);
            cart.ShippingAddress = null;

            var order = ShoppingService.CreateOrder(cart);

            var billingAddress = order.OriginalOrder.OrderBillingAddress;
            var shippingAddress = order.OriginalOrder.OrderShippingAddress;

            Assert.AreEqual(billingAddress.AddressPersonalName, shippingAddress.AddressPersonalName);
        }


        [TestCase(1, false)]
        [TestCase(20, true)]
        public void FreeShipping_RemainingAmountForFreeShipping_FreeShippingAppliedCorrectly(int itemUnits, bool freeShipping)
        {
            var cart = CreateEmptyShoppingCart();
            cart.AddItem(Factory.SKUAvailable.SKUID, itemUnits);
            cart.ShippingOption = Factory.ShippingOptionDefault;

            var service = new PricingService();

            decimal expectedAmount = (decimal)(ShippingDiscount.DiscountItemMinOrderAmount - cart.OriginalCart.TotalItemsPrice);
            expectedAmount = Math.Max(0, expectedAmount);

            decimal remainingAmount = service.CalculateRemainingAmountForFreeShipping(cart);
            bool cartHasFreeShipping = cart.Shipping == 0;

            CMSAssert.All(
                () => Assert.AreEqual(freeShipping, cartHasFreeShipping, "Free shipping applied incorrectly"),
                () => Assert.AreEqual(expectedAmount, remainingAmount, "Remaining amount does not match")
            );
        }


        private void CreateShippingDiscounts()
        {
            ShippingDiscount = Factory.NewShippingDiscount(100, false, 50);
            ShippingDiscount.Insert();
        }


        private class OrderInfoProviderWithMailLogging : OrderInfoProvider
        {
            public readonly List<string> EmailTemplatesSendToCustomer = new List<string>();
            public readonly List<string> EmailTemplatesSendToAdmin = new List<string>();


            protected override void SendEmailNotificationInternal(ShoppingCartInfo cartObj, string templateName, string defaultTemplate, string defaultSubject, bool toCustomer, string eventSource)
            {
                var log = toCustomer ? EmailTemplatesSendToCustomer : EmailTemplatesSendToAdmin;

                log.Add(string.IsNullOrEmpty(templateName) ? defaultTemplate : templateName);

                base.SendEmailNotificationInternal(cartObj, templateName, defaultTemplate, defaultSubject, toCustomer, eventSource);
            }
        }
    }
}
