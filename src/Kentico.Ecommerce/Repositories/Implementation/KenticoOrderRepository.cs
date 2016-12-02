using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides CRUD operations for orders.
    /// </summary>
    public class KenticoOrderRepository : IOrderRepository
    {
        private int SiteID
        {
            get
            {
                return SiteContext.CurrentSiteID;
            }
        }


        /// <summary>
        /// Returns an order with the specified identifier.
        /// </summary>
        /// <param name="orderId">Order's identifier.</param>
        /// <returns><see cref="Order"/> object representing an order with the specified identifier. Returns <c>null</c> if not found.</returns>
        public Order GetById(int orderId)
        {
            var orderInfo = OrderInfoProvider.GetOrderInfo(orderId);

            if (orderInfo == null ||  orderInfo.OrderSiteID != SiteID)
            {
                return null;
            }

            var order = new Order(orderInfo);
            return order;
        }


        /// <summary>
        /// Returns an enumerable collection of TopN orders of the given customer ordered by OrderDate descending.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <param name="count">Number of retrieved orders. Using 0 returns all records.</param>
        /// <returns>Collection of the customer's orders. See <see cref="Order"/> for detailed information.</returns>
        public IEnumerable<Order> GetByCustomerId(int customerId, int count = 0)
        {
            var orders = OrderInfoProvider.GetOrders(SiteID)
                .WhereEquals("OrderCustomerID", customerId)
                .TopN(count)
                .OrderByDescending(orderInfo => orderInfo.OrderDate)
                .Select(info => new Order(info))
                .ToList();                

            return orders;
        }
    }
}