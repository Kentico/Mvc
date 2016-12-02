using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides CRUD operations for payment methods.
    /// </summary>
    public class KenticoPaymentMethodRepository : IPaymentMethodRepository
    {
        private int SiteID
        {
            get
            {
                return SiteContext.CurrentSiteID;
            }
        }


        /// <summary>
        /// Returns a payment method with the specified identifier.
        /// </summary>
        /// <param name="paymentMethodId">Payment method's identifier.</param>
        /// <returns><see cref="PaymentOptionInfo"/> object representing a payment method with the specified identifier. Returns <c>null</c> if not found.</returns>
        public PaymentOptionInfo GetById(int paymentMethodId)
        {
            var paymentInfo = PaymentOptionInfoProvider.GetPaymentOptionInfo(paymentMethodId);

            if (paymentInfo?.PaymentOptionSiteID == SiteID)
            {
                return paymentInfo;
            }

            if (paymentInfo?.PaymentOptionSiteID == 0 && ECommerceSettings.AllowGlobalPaymentMethods(SiteID))
            {
                return paymentInfo;
            }

            return null;
        }


        /// <summary>
        /// Returns an enumerable collection of all enabled payment methods.
        /// </summary>
        /// <returns>Collection of enabled payment methods. See <see cref="PaymentOptionInfo"/> for detailed information.</returns>
        public IEnumerable<PaymentOptionInfo> GetAll()
        {
            return PaymentOptionInfoProvider.GetPaymentOptions(SiteID, true)
                .ToList();
        }
    }
}