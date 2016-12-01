using System.Collections.Generic;

using CMS.Ecommerce;

using Kentico.Core.DependencyInjection;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Interface for classes providing CRUD operations for shipping options.
    /// </summary>
    public interface IShippingOptionRepository : IRepository
    {
        /// <summary>
        /// Returns a shipping option with the specified identifier.
        /// </summary>
        /// <param name="shippingOptionId">Shipping option's identifier.</param>
        /// <returns><see cref="ShippingOptionInfo"/> object representing a shipping option with the specified identifier. Returns <c>null</c> if not found.</returns>
        ShippingOptionInfo GetById(int shippingOptionId);


        /// <summary>
        /// Returns an enumerable collection of all enabled shipping options.
        /// </summary>
        /// <returns>Collection of enabled shipping options. See <see cref="ShippingOptionInfo"/> for detailed information.</returns>
        IEnumerable<ShippingOptionInfo> GetAllEnabled();
    }
}