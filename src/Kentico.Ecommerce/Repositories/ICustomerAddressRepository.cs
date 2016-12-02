using System.Collections.Generic;

using Kentico.Core.DependencyInjection;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Interface for classes providing CRUD operations for customers' addresses.
    /// </summary>
    public interface ICustomerAddressRepository : IRepository
    {
        /// <summary>
        /// Returns a customer's address with the specified identifier.
        /// </summary>
        /// <param name="addressId">Identifier of the customer's address.</param>
        /// <returns>Customer's address with the specified identifier. Returns <c>null</c> if not found.</returns>
        CustomerAddress GetById(int addressId);


        /// <summary>
        /// Returns an enumerable collection of a customer's addresses.
        /// </summary>
        /// <param name="customerId">Customer's identifier..</param>
        /// <returns>Collection of the customer's addresses. See <see cref="CustomerAddress"/> for detailed information.</returns>
        IEnumerable<CustomerAddress> GetByCustomerId(int customerId);


        /// <summary>
        /// Saves a customer's address in the database.
        /// </summary>
        /// <param name="address"><see cref="CustomerAddress"/> object representing a customer's address that is updated.</param>
        void Upsert(CustomerAddress address);
    }
}