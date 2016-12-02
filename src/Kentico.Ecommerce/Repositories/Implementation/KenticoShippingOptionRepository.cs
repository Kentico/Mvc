using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides CRUD operations for shipping options.
    /// </summary>
    public class KenticoShippingOptionRepository : IShippingOptionRepository
    {
        private int SiteID
        {
            get
            {
                return SiteContext.CurrentSiteID;
            }
        }


        /// <summary>
        /// Returns a shipping option with the specified identifier.
        /// </summary>
        /// <param name="shippingOptionId">Shipping option's identifier.</param>
        /// <returns><see cref="ShippingOptionInfo"/> object representing a shipping option with the specified identifier. Returns <c>null</c> if not found.</returns>
        public ShippingOptionInfo GetById(int shippingOptionId)
        {
            var shippingInfo = ShippingOptionInfoProvider.GetShippingOptionInfo(shippingOptionId);

            if (shippingInfo == null || shippingInfo.ShippingOptionSiteID != SiteID)
            {
                return null;
            }

            return shippingInfo;
        }


        /// <summary>
        /// Returns an enumerable collection of all enabled shipping options.
        /// </summary>
        /// <returns>Collection of enabled shipping options. See <see cref="ShippingOptionInfo"/> for detailed information.</returns>
        public IEnumerable<ShippingOptionInfo> GetAllEnabled()
        {
            return ShippingOptionInfoProvider.GetShippingOptions(SiteID, true)
                .ToList();
        }
    }
}