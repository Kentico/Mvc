using System;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class KenticoOrderRepositoryTests : UnitTests
    {
        private IOrderRepository mRepository;

        private const int SITE_ID1 = 1;
        private const int SITE_ID2 = 2;

        private const int CUSTOMER_ID = 1;
        private const int NON_EXISTING_CUSTOMER_ID = 2;
        
        private const int ORDER_FIRST_SITE_ID1 = 1;
        private const int ORDER_FIRST_SITE_ID2 = 2;
        private const int ORDER_SECOND_SITE_ID = 3;
        private const int NON_EXISTING_ORDER_ID = 4;


        [SetUp]
        public void SetUp()
        {            
            SetUpSite();
            SetUpOrders();

            SiteContext.CurrentSite = SiteInfoProvider.GetSiteInfo(SITE_ID1);

            mRepository = new KenticoOrderRepository();
        }


        [Test]
        public void GetOder_ExistingOrder_ReturnsOrder()
        {
            var order = mRepository.GetById(ORDER_FIRST_SITE_ID1);
            CMSAssert.All(
                () => Assert.IsNotNull(order, "Order not found."),
                () => Assert.AreEqual(ORDER_FIRST_SITE_ID1, order.OrderID, "Incorrect order returned.")
            );
            
        }


        [Test]
        public void GetOder_NonExistingOrder_ReturnsNull()
        {
            var order = mRepository.GetById(NON_EXISTING_ORDER_ID);
            Assert.IsNull(order, "Non-existing order found.");
        }


        [Test]
        public void GetOder_ExistingOrderFromAnotherSite_ReturnsNull()
        {
            var order = mRepository.GetById(ORDER_SECOND_SITE_ID);
            Assert.IsNull(order, "Order from another site returned.");
        }


        [Test]
        public void GetCustomerOrders_ExistingCustomerWithoutTopNParameter_ReturnsAllOrdersFromCurrentSite()
        {
            var orders = mRepository.GetByCustomerId(CUSTOMER_ID);
            CMSAssert.All(
                () => Assert.IsNotNull(orders),
                () => Assert.IsNotEmpty(orders, "No orders found for customer."),
                () => Assert.AreEqual(2, orders.Count(), "Orders count is not 2 as expected.")
            );
        }


        [Test]
        public void GetCustomerOrders_ExistingCustomerWithTopNParameter_ReturnsOneNewestOrderFromCurrentSite()
        {
            var orders = mRepository.GetByCustomerId(CUSTOMER_ID, 1);
            CMSAssert.All(
                () => Assert.IsNotNull(orders),
                () => Assert.IsNotEmpty(orders, "No orders found for customer."),
                () => Assert.AreEqual(1, orders.Count(), "More than 1 order returned for customer."),
                () => Assert.AreEqual(ORDER_FIRST_SITE_ID2, orders.First().OrderID, "Order returned is not the newest one.")
            );
        }


        [Test]
        public void GetCustomerOrders_NonExistingCustomer_ReturnsEmptyList()
        {
            var orders = mRepository.GetByCustomerId(NON_EXISTING_CUSTOMER_ID);
            CMSAssert.All(
                () => Assert.IsNotNull(orders),
                () => Assert.IsEmpty(orders, "Orders returned for non-existing customer.")
            );
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


        private void SetUpOrders()
        {
            Fake<OrderInfo, OrderInfoProvider>().WithData(
                new OrderInfo
                {
                    OrderID = ORDER_FIRST_SITE_ID1,
                    OrderSiteID = SITE_ID1,
                    OrderCustomerID = CUSTOMER_ID,
                    OrderDate = new DateTime(2016,02,02)
                },
                new OrderInfo
                {
                    OrderID = ORDER_FIRST_SITE_ID2,
                    OrderSiteID = SITE_ID1,
                    OrderCustomerID = CUSTOMER_ID,
                    OrderDate = new DateTime(2016, 02, 03)
                },
                new OrderInfo
                {
                    OrderID = ORDER_SECOND_SITE_ID,
                    OrderSiteID = SITE_ID2,
                    OrderCustomerID = CUSTOMER_ID,
                    OrderDate = new DateTime(2016, 02, 01)
                });
        }
    }
}
