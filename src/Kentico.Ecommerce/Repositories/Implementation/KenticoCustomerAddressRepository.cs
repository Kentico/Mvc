using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides CRUD operations for customer addresses.
    /// </summary>
    public class KenticoCustomerAddressRepository : ICustomerAddressRepository
    {
        /// <summary>
        /// Returns a customer's address with the specified identifier.
        /// </summary>
        /// <param name="addressId">Identifier of the customer's address.</param>
        /// <returns>Customer's address with the specified identifier. Returns <c>null</c> if not found.</returns>
        public CustomerAddress GetById(int addressId)
        {
            var addressInfo = AddressInfoProvider.GetAddressInfo(addressId);

            if (addressInfo == null)
            {
                return null;
            }

            return new CustomerAddress(addressInfo);
        }


        /// <summary>
        /// Returns an enumerable collection of a customer's addresses.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <returns>Collection of customer's addresses. See <see cref="CustomerAddress"/> for detailed information.</returns>
        public IEnumerable<CustomerAddress> GetByCustomerId(int customerId)
        {
            return AddressInfoProvider.GetAddresses(customerId)
                .Select(info => new CustomerAddress(info))
                .ToList();
        }


        /// <summary>
        /// Saves a customer's address into the database.
        /// </summary>
        /// <param name="address"><see cref="CustomerAddress"/> object representing a customer's address that is inserted.</param>
        public void Upsert(CustomerAddress address)
        {
            AddressInfoProvider.SetAddressInfo(address.OriginalAddress);
        }
    }
}