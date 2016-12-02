using System.Collections.Generic;

using Kentico.Core.DependencyInjection;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Interface for classes providing CRUD operations for orders.
    /// </summary>
    public interface IOrderRepository : IRepository
    {
        /// <summary>
        /// Returns an order with the specified identifier.
        /// </summary>
        /// <param name="orderId">Order's identifier.</param>
        /// <returns><see cref="Order"/> object representing an order with the specified identifier. Returns <c>null</c> if not found.</returns>
        Order GetById(int orderId);


        /// <summary>
        /// Returns an enumerable collection of the given customer's orders.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <param name="count">Number of retrieved orders. Using 0 returns all records.</param>
        /// <returns>Collection of the customer's orders. See <see cref="Order"/> for detailed information.</returns>
        IEnumerable<Order> GetByCustomerId(int customerId, int count = 0);
    }
}