using System;

using CMS.Activities;
using CMS.Core;
using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using Kentico.Ecommerce.Tests.Fakes;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    public class OrderTests : EcommerceTestsBase
    {
        ShoppingService mService;

        OrderStatusInfo mOrderStatusNew,
                        mOrderStatusFailed,
                        mOrderStatusPaid;

        PaymentOptionInfo mPaymentOption;
        PaymentOptionInfo mPaymentOptionWithoutStatus;

        EcommerceActivitiesLoggerFake mEcommerceActivitiesLogger;


        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Service.Use<IActivityLogService, ActivityLogServiceFake>();
            mEcommerceActivitiesLogger = new EcommerceActivitiesLoggerFake();

            SetUpSite();

            SetUpOrderStatuses();
            SetUpPaymentOptions();

            mService = new ShoppingService(mEcommerceActivitiesLogger);
        }


        [SetUp]
        public void SetUp()
        {
            SiteContext.CurrentSite = SiteInfo;

            Service.Use<IActivityLogService, ActivityLogServiceFake>(Guid.NewGuid().ToString());

            // Setting order generates invoice which has a macro in the subject. The macro cannot be resolved and an error is logged.
            TestsEventLogProvider.FailForErrorEvent = false;
        }


        private void SetUpOrderStatuses()
        {
            mOrderStatusNew = Factory.DefaultOrderStatus;

            mOrderStatusFailed = Factory.NewOrderStatus(2);
            mOrderStatusFailed.Insert();

            mOrderStatusPaid = Factory.NewOrderStatus(3, true);
            mOrderStatusPaid.Insert();
        }


        private void SetUpPaymentOptions()
        {
            mPaymentOption = Factory.PaymentMethodDefault;
            mPaymentOption.PaymentOptionFailedOrderStatusID = mOrderStatusFailed.StatusID;
            mPaymentOption.PaymentOptionSucceededOrderStatusID = mOrderStatusPaid.StatusID;
            mPaymentOption.Update();

            mPaymentOptionWithoutStatus = Factory.NewPaymentOption();
            mPaymentOptionWithoutStatus.PaymentOptionFailedOrderStatusID = 0;
            mPaymentOptionWithoutStatus.PaymentOptionSucceededOrderStatusID = 0;
            mPaymentOptionWithoutStatus.Insert();
        }


        private Order CreateOrderFromNewCart()
        {
            var cartInfo = new ShoppingCartInfo
            {
                ShoppingCartSiteID = SiteInfo.SiteID,
                ShoppingCartCurrencyID = Factory.MainCurrency.CurrencyID,
                ShoppingCartPaymentOptionID = Factory.PaymentMethodDefault.PaymentOptionID,
                ShoppingCartCustomerID = Factory.CustomerAnonymous.CustomerID,
                ShoppingCartBillingAddress = Factory.CustomerAddressUSA
            };
            cartInfo.Evaluate();
            cartInfo.Insert();

            var cart = new ShoppingCart(cartInfo, mEcommerceActivitiesLogger, null, null);

            return mService.CreateOrder(cart);
        }


        [Test]
        public void SetAsPaid_OrderIsPaid()
        {
            var order = CreateOrderFromNewCart();

            order.SetAsPaid();

            CMSAssert.All(
                () => Assert.That(order.OriginalOrder.OrderIsPaid),
                () => Assert.That(order.IsPaid)
            );
        }


        [Test]
        public void SetPaymentResult_PaymentSuccess()
        {
            var order = CreateOrderFromNewCart();

            var result = new PaymentResultInfo
            {
                PaymentIsCompleted = true
            };

            order.SetPaymentResult(result);

            CMSAssert.All(
                () => Assert.IsTrue(order.IsPaid, "Order payment is successful but order is not marked as paid."),
                () => Assert.AreEqual(order.StatusID, Factory.PaymentMethodDefault.PaymentOptionSucceededOrderStatusID, "Order payment is successful but order status is not correctly updated."),
                () => Assert.NotNull(order.PaymentResult, "Order payment result is null.")
            );
        }


        [Test]
        public void SetPaymentResult_PaymentFailed()
        {
            var order = CreateOrderFromNewCart();

            var result = new PaymentResultInfo
            {
                PaymentIsCompleted = false,
                PaymentIsFailed = true
            };

            order.SetPaymentResult(result);

            CMSAssert.All(
                () => Assert.IsFalse(order.IsPaid, "Order payment failed but order is marked as paid."),
                () => Assert.AreEqual(order.StatusID, Factory.PaymentMethodDefault.PaymentOptionFailedOrderStatusID, "Order payment failed but order status is not correctly updated."),
                () => Assert.NotNull(order.PaymentResult, "Order payment result is null.")
            );
        }


        [Test]
        public void SetPaymentResultWithoutStatus_PaymentSuccess()
        {
            var order = CreateOrderFromNewCart();
            order.OriginalOrder.OrderPaymentOptionID = mPaymentOptionWithoutStatus.PaymentOptionID;

            var originalstatusId = order.StatusID;

            var result = new PaymentResultInfo
            {
                PaymentIsCompleted = true
            };

            order.SetPaymentResult(result);

            CMSAssert.All(
                () => Assert.IsTrue(order.IsPaid, "Order payment is successful but order is not marked as paid."),
                () => Assert.AreEqual(order.StatusID, originalstatusId, "Order payment was changed."),
                () => Assert.NotNull(order.PaymentResult, "Order payment result is null.")
            );
        }


        [Test]
        public void SetPaymentResultWithoutStatus_PaymentFailed()
        {
            var order = CreateOrderFromNewCart();
            order.OriginalOrder.OrderPaymentOptionID = mPaymentOptionWithoutStatus.PaymentOptionID;

            var originalstatusId = order.StatusID;

            var result = new PaymentResultInfo
            {
                PaymentIsCompleted = false,
                PaymentIsFailed = true
            };

            order.SetPaymentResult(result);

            CMSAssert.All(
                () => Assert.IsFalse(order.IsPaid, "Order payment failed but order is marked as paid."),
                () => Assert.AreEqual(order.StatusID, originalstatusId, "Order payment was changed."),
                () => Assert.NotNull(order.PaymentResult, "Order payment result is null.")
            );
        }


        [Test]
        public void SetPaymentResult_PaymentCaptured()
        {
            var order = CreateOrderFromNewCart();
            var originalStatusID = order.StatusID;

            var result = new PaymentResultInfo
            {
                PaymentIsCompleted = false,
                PaymentStatusName = "Money captured"
            };

            order.SetPaymentResult(result);

            CMSAssert.All(
                () => Assert.IsFalse(order.IsPaid, "Order payment is captured but order is marked as paid."),
                () => Assert.AreEqual(order.StatusID, originalStatusID, "Order status should not be updated"),
                () => Assert.NotNull(order.PaymentResult, "Order payment result is null.")
            );
        }


        [Test]
        public void SetPaymentResult_InvalidParameters()
        {
            var order = CreateOrderFromNewCart();

            var result = new PaymentResultInfo
            {
                PaymentIsCompleted = true,
                PaymentIsFailed = true
            };

            Assert.That(() => order.SetPaymentResult(result), Throws.Exception.TypeOf<InvalidOperationException>());
        }
    }
}
