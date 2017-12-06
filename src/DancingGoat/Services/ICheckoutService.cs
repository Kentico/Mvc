using System.Collections.Generic;

using CMS.Ecommerce;
using CMS.Globalization;

using DancingGoat.Models.Checkout;

using Kentico.Core.DependencyInjection;
using Kentico.Ecommerce;

namespace DancingGoat.Services
{
    /// <summary>
    /// Interface for service providing methods to build checkout models.
    /// </summary>
    public interface ICheckoutService : IService
    {
        /// <summary>
        /// Gets shipping option info.
        /// </summary>
        /// <param name="shippingOptionId">ID of shipping option which should be returned.</param>
        ShippingOptionInfo GetShippingOption(int shippingOptionId);


        /// <summary>
        /// Gets address with <paramref name="addressID"/> ID, but only if the address belongs to the customer used by shopping cart.
        /// </summary>
        /// <param name="addressID">The identifier of the address.</param>
        /// <returns>Address of the customer used by shopping cart. <c>Null</c> if customer does not have address with given Id.</returns>
        CustomerAddress GetCustomerAddress(int addressID);


        /// <summary>
        /// Gets payment method with required ID. 
        /// </summary>
        /// <param name="paymentMethodId">ID of payment method which should be returned.</param>
        PaymentOptionInfo GetPaymentMethod(int paymentMethodId);


        /// <summary>
        /// Create an enumerable collection of states for given country.
        /// </summary>
        /// <param name="countryId">Country ID which states should be listed.</param>
        IEnumerable<StateInfo> GetCountryStates(int countryId);


        /// <summary>
        /// Gets the address with the specified identifier.
        /// </summary>
        /// <param name="addressID">Address identifier.</param>
        CustomerAddress GetAddress(int addressID);


        /// <summary>
        /// Checks if coupon code value can be used during checkout process.
        /// </summary>
        /// <param name="couponCode">Coupon code to be checked.</param>
        /// <returns>True if coupon code defined in <paramref name="couponCode"/> is accepted by shopping cart or if coupon code is empty.</returns>
        bool IsCouponCodeValueValid(string couponCode);


        /// <summary>
        /// Checks if shipping option is among applicable shipping options for the shopping cart. 
        /// </summary>
        /// <param name="shippingOptionId">ID of shipping option.</param>
        /// <returns>True if shipping option can be used in the shopping cart.</returns>
        bool IsShippingOptionValid(int shippingOptionId);


        /// <summary>
        /// Checks if country exists.
        /// </summary>
        /// <param name="countryId">ID of country which should be checked</param>
        /// <returns>Return true if country exists.</returns>
        bool IsCountryValid(int countryId);


        /// <summary>
        /// Checks if state is valid for given country.
        /// </summary>
        /// <param name="countryId">ID of state`s country </param>
        /// <param name="stateId">ID of state</param>
        /// <returns>True if state is not required or state belongs to the country.</returns>
        bool IsStateValid(int countryId, int? stateId);


        /// <summary>
        /// Checks if payment method is applicable for given shopping cart.
        /// </summary>
        /// <param name="paymentMethodId">ID of payment method</param>
        /// <returns>True if payment method applicable for current shopping cart.</returns>
        bool IsPaymentMethodValid(int paymentMethodId);


        /// <summary>
        /// Creates view model for checkout delivery step.
        /// </summary>
        /// <param name="customer">Filled customer details</param>
        /// <param name="billingAddress">Filled billing address</param>
        /// <param name="shippingOption">Selected shipping option</param>
        DeliveryDetailsViewModel PrepareDeliveryDetailsViewModel(CustomerViewModel customer = null, BillingAddressViewModel billingAddress = null, ShippingOptionViewModel shippingOption = null);


        /// <summary>
        /// Creates view model for checkout preview step. 
        /// </summary>
        /// <param name="paymentMethod">Payment method selected on preview step</param>
        PreviewViewModel PreparePreviewViewModel(PaymentMethodViewModel paymentMethod = null);


        /// <summary>
        /// Creates view model for Shopping cart step.
        /// </summary>
        CartViewModel PrepareCartViewModel(IEnumerable<string> appliedCouponCodes = null);
    }
}