using System;
using System.Collections.Generic;
using System.Linq;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Membership;

using EcommerceActivityLogger = Kentico.Activities.EcommerceActivityLogger;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a customer's shopping cart.
    /// </summary>
    public class ShoppingCart
    {
        private readonly EcommerceActivityLogger mActivityLogger;
        private readonly ICurrentContactProvider mCurrentContactProvider;
        private IEnumerable<ShoppingCartItem> mItems;
        private Customer mCustomer;
        private CustomerAddress mBillingAddress;
        private CustomerAddress mShippingAddress;
        private Currency mCurrency;


        /// <summary>
        /// Original Kentico shopping cart info object from which the model gathers information. See <see cref="ShoppingCartInfo"/> for detailed information.
        /// </summary>
        internal readonly ShoppingCartInfo OriginalCart;


        /// <summary>
        /// List of shopping cart items. See <see cref="ShoppingCartItem"/> for detailed information.
        /// </summary>
        public IEnumerable<ShoppingCartItem> Items => mItems ?? (mItems = OriginalCart.CartItems.Select(i => new ShoppingCartItem(i)));


        /// <summary>
        /// Indicates whether shopping cart contains no items.
        /// </summary>
        public bool IsEmpty => OriginalCart.IsEmpty;


        /// <summary>
        /// Gets the total price of the shopping cart. All discounts except for the order discount, taxes and shipping costs are included.
        /// </summary>
        public decimal TotalPrice => OriginalCart.TotalPrice;


        /// <summary>
        /// Gets the grand total of the shopping cart. All discount, taxes and shipping costs are included.
        /// </summary>
        public decimal GrandTotal => OriginalCart.GrandTotal;


        /// <summary>
        /// Gets the total tax which is applied to all shopping cart items altogether.
        /// </summary>
        public decimal TotalTax => OriginalCart.TotalTax;


        /// <summary>
        /// Indicates whether the shopping cart has a shipping address filled.
        /// </summary>
        public bool HasShippingAddress => OriginalCart.ShoppingCartShippingAddress != null;
        

        /// <summary>
        /// Total shipping costs.
        /// </summary>
        public decimal Shipping => OriginalCart.TotalShipping;
        

        /// <summary>
        /// Indicates if shipping is needed for the shopping cart.
        /// </summary>
        public bool IsShippingNeeded => OriginalCart.IsShippingNeeded;
        

        /// <summary>
        /// Customer's coupon codes applied to shopping cart.
        /// </summary>
        public IEnumerable<string> AppliedCouponCodes => OriginalCart.CouponCodes.AllAppliedCodes.Select(x => x.Code);


        /// <summary>
        /// Owner of the shopping cart. Returns <c>null</c> if the customer is anonymous. See <see cref="UserInfo"/> for detailed information.
        /// </summary>
        public UserInfo User
        {
            get
            {
                return OriginalCart.User;
            }
            set
            {
                OriginalCart.User = value;
            }
        }


        /// <summary>
        /// The currency of the shopping cart.
        /// </summary>
        public Currency Currency => mCurrency ?? (mCurrency = new Currency(OriginalCart.Currency));


        /// <summary>
        /// Selected payment method. If none is selected and there is only one payment method available, it is selected automatically. See <see cref="PaymentOptionInfo"/> for detailed information.
        /// </summary>
        public PaymentOptionInfo PaymentMethod
        {
            get
            {
                return OriginalCart.PaymentOption;
            }
            set
            {
                if (value != null)
                {
                    OriginalCart.ShoppingCartPaymentOptionID = value.PaymentOptionID;
                }
            }
        }


        /// <summary>
        /// Selected shipping option. If none is selected and only one is available, it is selected automatically. See <see cref="ShippingOptionInfo"/> for detailed information.
        /// </summary>
        public ShippingOptionInfo ShippingOption
        {
            get
            {
                return OriginalCart.ShippingOption;
            }
            set
            {
                if (value != null)
                {
                    OriginalCart.ShoppingCartShippingOptionID = value.ShippingOptionID;
                }
            }
        }


        /// <summary>
        /// Customer's information. See <see cref="Customer"/> for detailed information.
        /// </summary>
        public Customer Customer
        {
            get
            {
                if ((mCustomer == null) && ((OriginalCart.Customer != null) || OriginalCart.User != null))
                {
                    mCustomer = new Customer(GetCustomerInfo());
                }

                return mCustomer;
            }
            set
            {
                if (value?.ID != mCustomer?.ID)
                {
                    mCustomer = value;
                }
            }
        }


        /// <summary>
        /// Shopping cart's entered customer billing address. See <see cref="CustomerAddress"/> for detailed information.
        /// </summary>
        public CustomerAddress BillingAddress
        {
            get
            {
                if ((mBillingAddress == null) && (OriginalCart.ShoppingCartBillingAddress != null))
                {
                    mBillingAddress = new CustomerAddress((AddressInfo)OriginalCart.ShoppingCartBillingAddress);
                }

                return mBillingAddress;
            }
            set
            {
                mBillingAddress = value;
            }
        }


        /// <summary>
        /// Shopping cart's entered customer shipping address. See <see cref="CustomerAddress"/> for detailed information.
        /// </summary>
        public CustomerAddress ShippingAddress
        {
            get
            {
                if ((mShippingAddress == null) && (OriginalCart.ShoppingCartShippingAddress != null))
                {
                    mShippingAddress = new CustomerAddress((AddressInfo)OriginalCart.ShoppingCartShippingAddress);
                }

                return mShippingAddress;
            }
            set
            {
                mShippingAddress = value;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCart"/> class.
        /// </summary>
        /// <param name="originalCart"><see cref="ShoppingCartInfo"/> object representing an original Kentico shopping cart info object from which the model is created.</param>
        public ShoppingCart(ShoppingCartInfo originalCart)
            :this(originalCart, new EcommerceActivityLogger(), Service.Resolve<ICurrentContactProvider>())
        {
        }


        internal ShoppingCart(ShoppingCartInfo originalCart, EcommerceActivityLogger activityLogger, ICurrentContactProvider currentContactProvider)
        {
            if (originalCart == null)
            {
                throw new ArgumentNullException(nameof(originalCart));
            }

            OriginalCart = originalCart;
            mActivityLogger = activityLogger;
            mCurrentContactProvider = currentContactProvider;
        }


        /// <summary>
        /// Checks the shopping cart content.
        /// </summary>
        /// <remarks>
        /// The following conditions must be met to pass the check:
        /// 1) All shopping cart items are enabled.
        /// 2) Max. number of units in one order is not exceeded.
        /// 3) There is enough units in the inventory.
        /// </remarks>
        /// <returns>Container with check results. See <see cref="ShoppingCartCheckResult"/> for detailed information.</returns>
        public ShoppingCartCheckResult ValidateContent() => ShoppingCartInfoProvider.CheckShoppingCart(OriginalCart);


        /// <summary>
        /// Checks if the payment method is applicable for this shopping cart.
        /// </summary>
        /// <param name="paymentMethod">Payment method which is checked.</param>
        /// <returns>By default, returns always <c>true</c>. You can override the <see cref="PaymentOptionInfoProvider.IsPaymentOptionApplicable"/> method to change this behavior.</returns>
        public bool IsPaymentMethodApplicable(PaymentOptionInfo paymentMethod)
            => PaymentOptionInfoProvider.IsPaymentOptionApplicable(OriginalCart, paymentMethod);


        /// <summary>
        /// Adds an item to the shopping cart and saves the newly created cart into the database. Also saves the shopping cart if it wasn't saved yet.
        /// </summary>
        /// <param name="skuId">ID of the added product's SKU object.</param>
        /// <param name="quantity">Number of added product units.</param>
        public void AddItem(int skuId, int quantity = 1)
        {
            var parameters = new ShoppingCartItemParameters
            {
                SKUID = skuId,
                Quantity = quantity
            };

            var cartItem = ShoppingCartInfoProvider.SetShoppingCartItem(OriginalCart, parameters);
            if (cartItem == null)
            {
                return;
            }

            if (OriginalCart.ShoppingCartID == 0)
            {
                Save(false);
            }

            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);

            mActivityLogger?.LogProductAddedToShoppingCartActivity(cartItem.SKU, quantity);

            mItems = null;
        }


        /// <summary>
        /// Updates product units in the shopping cart and saves the item change into the database.
        /// </summary>
        /// <param name="itemId">ID of the shopping cart item.</param>
        /// <param name="quantity">New number of the product units.</param>
        public void UpdateQuantity(int itemId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ID == itemId);
            if (item == null)
            {
                return;
            }

            if (quantity == 0)
            {
                RemoveItem(item.ID);
                return;
            }
            
            ShoppingCartItemInfoProvider.UpdateShoppingCartItemUnits(item.OriginalCartItem, quantity);

            mItems = null;
        }


        /// <summary>
        /// Removes an item from the shopping cart. Also deletes the cart item from the database.
        /// </summary>
        /// <remarks>
        /// If the cart item has already been removed and does not exist in the cart, no activity is logged.
        /// </remarks>
        /// <param name="itemId">ID of the shopping cart item.</param>
        public void RemoveItem(int itemId)
        {
            var item = Items.FirstOrDefault(i => i.ID == itemId);
            if (item == null)
            {
                return;
            }

            var originalItem = item.OriginalCartItem;
            ShoppingCartInfoProvider.RemoveShoppingCartItem(OriginalCart, originalItem.CartItemID);
            
            mActivityLogger?.LogProductRemovedFromShoppingCartActivity(originalItem.SKU ,originalItem.CartItemUnits);
            ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(originalItem);
            
            mItems = null;
        }


        /// <summary>
        /// Removes all items from the shopping cart.
        /// </summary>
        public void RemoveAllItems()
        {
            OriginalCart.CartItems.ForEach(cartItem =>
            {
                mActivityLogger?.LogProductRemovedFromShoppingCartActivity(cartItem.SKU, cartItem.CartItemUnits);
            });

            ShoppingCartInfoProvider.EmptyShoppingCart(OriginalCart);
            mItems = null;
        }
        

        /// <summary>
        /// Validates the shopping cart object.
        /// </summary>
        /// <returns>Instance of the <see cref="ShoppingCartValidator"/> with validation summary.</returns>
        public ShoppingCartValidator Validate()
        {
            var validator = new ShoppingCartValidator(this);
            validator.Validate();
            return validator;
        }


        /// <summary>
        /// Evaluates the price information.
        /// </summary>
        public void Evaluate()
        {
            OriginalCart.Evaluate();
        }


        /// <summary>
        /// Saves the shopping cart into the database.
        /// </summary>
        /// <param name="validate">Specifies whether the validation should be performed.</param>
        public void Save(bool validate = true)
        {
            if (validate)
            {
                var result = Validate();
                if (result.CheckFailed)
                {
                    throw new InvalidOperationException();
                }
            }

            SaveCustomer();
            SaveAddresses();

            ShoppingCartInfoProvider.SetShoppingCartInfo(OriginalCart);
            foreach (var item in OriginalCart.CartItems)
            {
                item.ShoppingCart = OriginalCart;
                item.ShoppingCartID = OriginalCart.ShoppingCartID;
                ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(item);
            }
        }


        /// <summary>
        /// Add the specified coupon code to the shopping cart.
        /// </summary>
        /// <param name="couponCode">Coupon code to add</param>
        public bool AddCouponCode(string couponCode)
        {
            return OriginalCart.AddCouponCode(couponCode);
        }


        /// <summary>
        /// Remove specified coupon code from the shopping cart.
        /// </summary>
        /// <param name="couponCode">Coupon code to remove</param>
        public void RemoveCouponCode(string couponCode)
        {
            OriginalCart.RemoveCouponCode(couponCode);
        }


        /// <summary>
        /// Saves the billing and shipping address into the database (if addresses are available).
        /// </summary>
        private void SaveAddresses()
        {
            SaveAddress(BillingAddress);
            SaveAddress(ShippingAddress);

            OriginalCart.ShoppingCartBillingAddress = BillingAddress?.OriginalAddress;
            OriginalCart.ShoppingCartShippingAddress = ShippingAddress?.OriginalAddress;
        }


        private void SaveAddress(CustomerAddress address)
        {
            if (address != null)
            {
                address.OriginalAddress.AddressCustomerID = Customer.ID;
                address.Save(false);
            }
        }


        /// <summary>
        /// Saves the shopping cart's customer into the database.
        /// </summary>
        private void SaveCustomer()
        {
            if (Customer == null)
            {
                return;
            }

            var customerInfo = Customer.OriginalCustomer;
            if (!customerInfo.CustomerIsRegistered)
            {
                customerInfo.CustomerSiteID = OriginalCart.ShoppingCartSiteID;
            }

            Customer.Save(false);
            SetCustomerRelationAndUpdateContact(customerInfo);
            OriginalCart.Customer = customerInfo;
        }


        private CustomerInfo GetCustomerInfo() => OriginalCart.Customer ?? CreateCustomerFromUser();
        

        private CustomerInfo CreateCustomerFromUser()
        {
            var user = OriginalCart.User;
            if (user == null)
            {
                return null;
            }

            return new CustomerInfo
            {
                CustomerEmail = user.Email,
                CustomerFirstName = user.FirstName,
                CustomerLastName = user.LastName,
                CustomerUserID = user.UserID,
                CustomerPhone = user.UserSettings.UserPhone
            };
        }


        private void SetCustomerRelationAndUpdateContact(CustomerInfo customerInfo)
        {
            if (mCurrentContactProvider == null)
            {
                return;
            }

            var currentContact = mCurrentContactProvider.GetCurrentContact(MembershipContext.AuthenticatedUser, false);
            mCurrentContactProvider.SetCurrentContact(currentContact);

            Service.Resolve<IContactRelationAssigner>().Assign(MemberTypeEnum.EcommerceCustomer, customerInfo, currentContact);
            ContactInfoProvider.UpdateContactFromExternalData(
                customerInfo,
                DataClassInfoProvider.GetDataClassInfo(CustomerInfo.TYPEINFO.ObjectClassName).ClassContactOverwriteEnabled,
                currentContact.ContactID);
        }
    }
}
