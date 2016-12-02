using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Base;
using CMS.Ecommerce;
using CMS.Globalization;

using DancingGoat.Models.Checkout;
using DancingGoat.Repositories;

using Kentico.Ecommerce;

namespace DancingGoat.Services
{
    /// <summary>
    /// Provides methods to build checkout models.
    /// </summary>
    public class CheckoutService : ICheckoutService
    {
        private readonly IShoppingService mShoppingService;
        private readonly IPricingService mPricingService;
        private readonly IPaymentMethodRepository mPaymentMethodRepository;
        private readonly IShippingOptionRepository mShippingOptionRepository;
        private readonly ICountryRepository mCountryRepository;
        private readonly ICustomerAddressRepository mAddressRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shoppingService">Shopping service</param>
        /// <param name="pricingService">Pricing service</param>
        /// <param name="addressRepository">Address repository</param>
        /// <param name="paymentMethodRepository">Payment method repository</param>
        /// <param name="shippingOptionRepository">Shipping option repository</param>
        /// <param name="countryRepository">Country repository</param>
        public CheckoutService(IShoppingService shoppingService, IPricingService pricingService, ICustomerAddressRepository addressRepository, IPaymentMethodRepository paymentMethodRepository, IShippingOptionRepository shippingOptionRepository, ICountryRepository countryRepository)
        {
            mShoppingService = shoppingService;
            mPricingService = pricingService;
            mPaymentMethodRepository = paymentMethodRepository;
            mShippingOptionRepository = shippingOptionRepository;
            mCountryRepository = countryRepository;
            mAddressRepository = addressRepository;
        }

        
        /// <summary>
        /// Gets shipping option info.
        /// </summary>
        /// <param name="shippingOptionId">ID of shipping option which should be returned.</param>
        public ShippingOptionInfo GetShippingOption(int shippingOptionId)
        {
            return mShippingOptionRepository.GetById(shippingOptionId);
        }


        /// <summary>
        /// Gets address with <paramref name="addressID"/> ID, but only if the address belongs to the customer used by shopping cart.
        /// </summary>
        /// <param name="addressID">The identifier of the address.</param>
        /// <returns>Address of the customer used by shopping cart. <c>Null</c> if customer does not have address with given Id.</returns>
        public CustomerAddress GetCustomerAddress(int addressID)
        {
            var customer = mShoppingService.GetCurrentShoppingCart().Customer;
            return mAddressRepository.GetByCustomerId(customer.ID).FirstOrDefault(a => a.ID == addressID);
        }


        /// <summary>
        /// Gets payment method with required ID. 
        /// </summary>
        /// <param name="paymentMethodId">ID of payment method which should be returned.</param>
        public PaymentOptionInfo GetPaymentMethod(int paymentMethodId)
        {
            return mPaymentMethodRepository.GetById(paymentMethodId);
        }


        /// <summary>
        /// Create an enumerable collection of states for given country.
        /// </summary>
        /// <param name="countryId">Country ID which states should be listed.</param>
        public IEnumerable<StateInfo> GetCountryStates(int countryId)
        {
            return mCountryRepository.GetCountryStates(countryId).ToList();
        }


        /// <summary>
        /// Gets the address with the specified identifier.
        /// </summary>
        /// <param name="addressID">Address identifier.</param>
        public CustomerAddress GetAddress(int addressID)
        {
            return mAddressRepository.GetById(addressID);
        }
        

        /// <summary>
        /// Checks if coupon code value can be used during checkout process.
        /// </summary>
        /// <param name="couponCode">Coupon code to be checked.</param>
        /// <returns>True if coupon code defined in <paramref name="couponCode"/> is accepted by shopping cart or if coupon code is empty.</returns>
        public bool IsCouponCodeValueValid(string couponCode)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var assignedCode = cart.CouponCode;

            return string.IsNullOrEmpty(couponCode) || (cart.HasUsableCoupon && assignedCode.EqualsCSafe(couponCode)); 
        }


        /// <summary>
        /// Checks if shipping option is among applicable shipping options for the shopping cart. 
        /// </summary>
        /// <param name="shippingOptionId">ID of shipping option.</param>
        /// <returns>True if shipping option can be used in the shopping cart.</returns>
        public bool IsShippingOptionValid(int shippingOptionId)
        {
            var shippingOptions = mShippingOptionRepository.GetAllEnabled().ToList();

            return shippingOptions.Exists(s => s.ShippingOptionID == shippingOptionId);
        }


        /// <summary>
        /// Checks if country exists.
        /// </summary>
        /// <param name="countryId">ID of country which should be checked</param>
        /// <returns>Return true if country exists.</returns>
        public bool IsCountryValid(int countryId)
        {
            return mCountryRepository.GetCountry(countryId) != null;
        }


        /// <summary>
        /// Checks if state is valid for given country.
        /// </summary>
        /// <param name="countryId">ID of state`s country </param>
        /// <param name="stateId">ID of state</param>
        /// <returns>True if state is not required or state belongs to the country.</returns>
        public bool IsStateValid(int countryId, int? stateId)
        {
            var states = mCountryRepository.GetCountryStates(countryId).ToList();

            return (states.Count < 1) || states.Exists(s => s.StateID == stateId);
        }


        /// <summary>
        /// Checks if payment method is applicable for given shopping cart.
        /// </summary>
        /// <param name="paymentMethodId">ID of payment method</param>
        /// <returns>True if payment method applicable for current shopping cart.</returns>
        public bool IsPaymentMethodValid(int paymentMethodId)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var paymentMethods = GetApplicablePaymentMethods(cart).ToList();

            return paymentMethods.Exists(p => p.PaymentOptionID == paymentMethodId);
        }

        
        /// <summary>
        /// Creates view model for checkout delivery step.
        /// </summary>
        /// <param name="customer">Filled customer details</param>
        /// <param name="billingAddress">Filled billing address</param>
        /// <param name="shippingOption">Selected shipping option</param>
        public DeliveryDetailsViewModel PrepareDeliveryDetailsViewModel(CustomerViewModel customer = null, BillingAddressViewModel billingAddress = null, ShippingOptionViewModel shippingOption = null)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var countries = CreateCountryList();
            var shippingOptions = CreateShippingOptionList();

            customer = customer ?? new CustomerViewModel(cart.Customer);

            var addresses = (cart.Customer != null)
                ? mAddressRepository.GetByCustomerId(cart.Customer.ID)
                : Enumerable.Empty<CustomerAddress>();

            var billingAddresses = new SelectList(addresses, "ID", "Name");

            billingAddress = billingAddress ?? new BillingAddressViewModel(cart.BillingAddress, countries, billingAddresses);
            shippingOption = shippingOption ?? new ShippingOptionViewModel(cart.ShippingOption, shippingOptions);

            billingAddress.BillingAddressCountryStateSelector.Countries = billingAddress.BillingAddressCountryStateSelector.Countries ?? countries;
            billingAddress.BillingAddressSelector = billingAddress.BillingAddressSelector ?? new AddressSelectorViewModel { Addresses = billingAddresses };
            shippingOption.ShippingOptions = shippingOptions;

            var viewModel = new DeliveryDetailsViewModel
            {
                Customer = customer,
                BillingAddress = billingAddress,
                ShippingOption = shippingOption
            };

            return viewModel;
        }


        /// <summary>
        /// Creates view model for checkout preview step. 
        /// </summary>
        /// <param name="paymentMethod">Payment method selected on preview step</param>
        public PreviewViewModel PreparePreviewViewModel(PaymentMethodViewModel paymentMethod = null)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();
            var billingAddress = cart.BillingAddress;
            var shippingOption = cart.ShippingOption;
            var paymentMethods = CreatePaymentMethodList(cart);

            paymentMethod = paymentMethod ?? new PaymentMethodViewModel(cart.PaymentMethod, paymentMethods);

            // PaymentMethods are excluded from automatic binding and must be recreated manually after post action
            paymentMethod.PaymentMethods = paymentMethod.PaymentMethods ?? paymentMethods;

            var deliveryDetailsModel = new DeliveryDetailsViewModel
            {
                Customer = new CustomerViewModel(cart.Customer),
                BillingAddress = new BillingAddressViewModel(billingAddress, null),
                ShippingOption = new ShippingOptionViewModel(shippingOption, null)
            };

            var cartModel = new CartViewModel { Cart = cart };

            var viewModel = new PreviewViewModel
            {
                CartModel = cartModel,
                DeliveryDetails = deliveryDetailsModel,
                ShippingName = shippingOption.ShippingOptionDisplayName,
                PaymentMethod = paymentMethod
            };

            return viewModel;
        }


        /// <summary>
        /// Creates view model for Shopping cart step.
        /// </summary>
        public CartViewModel PrepareCartViewModel(string couponCode = null)
        {
            var cart = mShoppingService.GetCurrentShoppingCart();

            return new CartViewModel
            {
                Cart = cart,
                RemainingAmountForFreeShipping = mPricingService.CalculateRemainingAmountForFreeShipping(cart),
                CouponCode = couponCode ?? cart.CouponCode
            };
        }

        
        private SelectList CreatePaymentMethodList(ShoppingCart cart)
        {
            var paymentMethods = GetApplicablePaymentMethods(cart);

            return new SelectList(paymentMethods, "PaymentOptionID", "PaymentOptionDisplayName");
        }


        private IEnumerable<PaymentOptionInfo> GetApplicablePaymentMethods(ShoppingCart cart)
        {
            return mPaymentMethodRepository.GetAll().Where(cart.IsPaymentMethodApplicable);
        }
        

        private SelectList CreateCountryList()
        {
            var allCountries = mCountryRepository.GetAllCountries();
            return new SelectList(allCountries, "CountryID", "CountryDisplayName");
        }


        private SelectList CreateShippingOptionList()
        {
            var shippingOptions = mShippingOptionRepository.GetAllEnabled();
            var cart = mShoppingService.GetCurrentShoppingCart();

            var selectList = shippingOptions.Select(s =>
            {
                var shippingPrice = mPricingService.CalculateShippingOptionPrice(s, cart);
                var currency = shippingPrice.Currency;

                return new SelectListItem
                {
                    Value = s.ShippingOptionID.ToString(),
                    Text = $"{s.ShippingOptionDisplayName} ({currency.FormatPrice(shippingPrice.Price)})"
                };
            });

            return new SelectList(selectList, "Value", "Text");
        }
    }
}