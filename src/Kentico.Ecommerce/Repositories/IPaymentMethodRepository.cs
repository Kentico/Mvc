using System.Collections.Generic;

using CMS.Ecommerce;

using Kentico.Core.DependencyInjection;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Interface for classes providing CRUD operations for payment methods.
    /// </summary>
    public interface IPaymentMethodRepository : IRepository
    {
        /// <summary>
        /// Returns a payment method with the specified identifier.
        /// </summary>
        /// <param name="paymentMethodId">Payment method's identifier.</param>
        /// <returns><see cref="PaymentOptionInfo"/> object representing a payment method with the specified identifier. Returns <c>null</c> if not found.</returns>
        PaymentOptionInfo GetById(int paymentMethodId);


        /// <summary>
        /// Returns an enumerable collection of all enabled payment methods.
        /// </summary>
        /// <returns>Collection of enabled payment methods. See <see cref="PaymentOptionInfo"/> for detailed information.</returns>
        IEnumerable<PaymentOptionInfo> GetAll();
    }
}