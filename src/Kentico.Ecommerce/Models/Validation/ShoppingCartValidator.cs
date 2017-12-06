namespace Kentico.Ecommerce
{
    /// <summary>
    /// Class for shopping cart validation.
    /// </summary>
    public class ShoppingCartValidator
    {
        private readonly ShoppingCart mCart;


        /// <summary>
        /// Indicates if some validation failed.
        /// </summary>
        public bool CheckFailed => UserDisabled || PaymentMethodDisabled || PaymentMethodFromDifferentSite
            || ShoppingOptionDisabled || ShippingOptionFromDifferentSite
            || ((BillingAddress != null) && BillingAddress.CheckFailed)
            || ((ShippingAddress != null) && ShippingAddress.CheckFailed)
            || BillingAddressFromDifferentCustomer
            || ShippingAddressFromDifferentCustomer;


        /// <summary>
        /// True when user is not enabled.
        /// </summary>
        public bool UserDisabled { get; private set; }


        /// <summary>
        /// True when payment method is not enabled.
        /// </summary>
        public bool PaymentMethodDisabled { get; private set; }


        /// <summary>
        /// True when shipping option is not enabled.
        /// </summary>
        public bool ShoppingOptionDisabled { get; private set; }


        /// <summary>
        /// True when payment method is not available on the current site.
        /// </summary>
        public bool PaymentMethodFromDifferentSite { get; private set; }


        /// <summary>
        /// True when shipping option is not available on the current site.
        /// </summary>
        public bool ShippingOptionFromDifferentSite { get; private set; }


        /// <summary>
        /// True when billing address does not belong to the cart customer.
        /// </summary>
        public bool BillingAddressFromDifferentCustomer { get; private set; }


        /// <summary>
        /// True when shipping address does not belong to the cart customer.
        /// </summary>
        public bool ShippingAddressFromDifferentCustomer { get; private set; }


        /// <summary>
        /// Billing address validation results.
        /// </summary>
        public CustomerAddressValidator BillingAddress { get; private set; }


        /// <summary>
        /// Shipping address validation results.
        /// </summary>
        public CustomerAddressValidator ShippingAddress { get; private set; }


        /// <summary>
        /// Customer validation results.
        /// </summary>
        public CustomerValidator Customer { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartValidator"/> class.
        /// </summary>
        /// <param name="cart"><see cref="ShoppingCart"/> object representing a shopping cart that is validated.</param>
        public ShoppingCartValidator(ShoppingCart cart)
        {
            mCart = cart;
        }


        /// <summary>
        /// Validates the shopping cart.
        /// </summary>
        /// <remarks>
        /// The following conditions must be met to pass the validation:
        /// 1) User is enabled and is available on the current site.
        /// 2) Payment method is enabled and is available on the current site.
        /// 3) Shopping option is enabled and is available on the current site.
        /// 4) Billing and shipping addresses belong to the cart customer.
        /// 5) Billing and shipping addresses are both valid.
        /// </remarks>
        public void Validate()
        {
            ValidateUser();
            ValidateCustomer();
            ValidateAddresses();
            ValidatePaymentMethod();
            ValidateShippingOption();
        }


        private void ValidateUser()
        {
            if (mCart.User != null)
            {
                UserDisabled = !mCart.User.Enabled;
            }
        }


        private void ValidateCustomer()
        {
            if (mCart.Customer != null)
            {
                Customer = mCart.Customer.Validate();
            }
        }


        private void ValidatePaymentMethod()
        {
            if (mCart.PaymentMethod != null)
            {
                PaymentMethodDisabled = !mCart.PaymentMethod.PaymentOptionEnabled;
                PaymentMethodFromDifferentSite = !IsValidSiteID(mCart.PaymentMethod.PaymentOptionSiteID);
            }
        }


        private void ValidateShippingOption()
        {
            if (mCart.ShippingOption != null)
            {
                ShoppingOptionDisabled = !mCart.ShippingOption.ShippingOptionEnabled;
                ShippingOptionFromDifferentSite = !IsValidSiteID(mCart.ShippingOption.ShippingOptionSiteID);
            }
        }


        private void ValidateAddresses()
        {
            // Customer was not stored into the database yet
            if ((mCart.Customer == null) || (mCart.Customer.ID <= 0))
            {
                return;
            }

            // Validate billing address
            var validationResult = ValidateAddress(mCart.BillingAddress);
            BillingAddress = validationResult.validationResult;
            BillingAddressFromDifferentCustomer = validationResult.isFromDifferentCustomer;

            // Validate shipping address
            validationResult = ValidateAddress(mCart.ShippingAddress);
            ShippingAddress = validationResult.validationResult;
            ShippingAddressFromDifferentCustomer = validationResult.isFromDifferentCustomer;
        }
        

        private AddressValidationResult ValidateAddress(CustomerAddress address)
        {
            var result = new AddressValidationResult();

            if (address != null)
            {
                result.validationResult = address.Validate();
                
                if (address.ID > 0)
                {
                    result.isFromDifferentCustomer = address.OriginalAddress.AddressCustomerID != mCart.Customer.ID;
                }
            }

            return result;
        }


        private bool IsValidSiteID(int siteId)
        {
            return (siteId == 0) || (mCart.OriginalCart.ShoppingCartSiteID == siteId);
        }


        private class AddressValidationResult
        {
            internal CustomerAddressValidator validationResult;
            internal bool isFromDifferentCustomer;
        }
    }
}
