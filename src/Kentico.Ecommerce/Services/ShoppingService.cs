using CMS.Ecommerce;
using CMS.SiteProvider;

using EcommerceActivityLogger = Kentico.Activities.EcommerceActivityLogger;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Provides methods for managing shopping carts.
    /// </summary>
    public class ShoppingService : IShoppingService
    {
        private readonly Activities.EcommerceActivityLogger mActivityLogger = new EcommerceActivityLogger();


        /// <summary>
        /// Gets site name.
        /// </summary>
        protected string SiteName
        {
            get
            {
                return SiteContext.CurrentSiteName;
            }
        }

        
        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingService"/> class.
        /// </summary>
        public ShoppingService()
        {
        }


        internal ShoppingService(EcommerceActivityLogger activityLogger)
            :this()
        {
            mActivityLogger = activityLogger;
        }


        /// <summary>
        /// Returns the current shopping cart.
        /// </summary>
        /// <returns><see cref="ShoppingCart"/> object representing the shopping cart on the current site.</returns>
        public virtual ShoppingCart GetCurrentShoppingCart()
        {
            var cart = ECommerceContext.CurrentShoppingCart;

            return new ShoppingCart(cart);
        }


        /// <summary>
        /// Creates a new order from the shopping cart. Saves the shopping cart before the order creation.
        /// </summary>
        /// <param name="cart">Validated shopping cart (<see cref="ShoppingCart"/>) from which an order is created.</param>
        /// <returns><see cref="Order"/> object representing the created order.</returns>
        public virtual Order CreateOrder(ShoppingCart cart)
        {
            cart.Save();
            ShoppingCartInfoProvider.SetOrder(cart.OriginalCart);

            var orderInfo = cart.OriginalCart.Order;

            if (orderInfo != null)
            {
                SendOrderNotifications(cart);
                LogPurchaseActivities(cart);
            }

            return new Order(orderInfo);
        }


        /// <summary>
        /// Deletes a shopping cart from the database.
        /// </summary>
        /// <param name="cart">Shopping cart (<see cref="ShoppingCart"/>) that is deleted.</param>
        public virtual void DeleteShoppingCart(ShoppingCart cart)
        {
            ShoppingCartInfoProvider.DeleteShoppingCartInfo(cart.OriginalCart);
        }


        /// <summary>
        /// Gets the current customer.
        /// </summary>
        /// <returns><see cref="Customer"/> object representing the current customer. Returns <c>null</c> if there is not any current customer.</returns>
        public Customer GetCurrentCustomer()
        {
            var customerInfo = ECommerceContext.CurrentCustomer;
            return customerInfo == null
                ? null
                : new Customer(customerInfo);
        }


        private void SendOrderNotifications(ShoppingCart cart)
        {
            if (ECommerceSettings.SendOrderNotification(SiteName))
            {
                OrderInfoProvider.SendOrderNotificationToAdministrator(cart.OriginalCart);
                OrderInfoProvider.SendOrderNotificationToCustomer(cart.OriginalCart);
            }
        }


        private void LogPurchaseActivities(ShoppingCart cart)
        {
            var orderInfo = cart.OriginalCart.Order;

            var mainCurrency = new Currency(CurrencyInfoProvider.GetMainCurrency(SiteContext.CurrentSiteID));

            var priceString = mainCurrency.FormatPrice(orderInfo.OrderTotalPriceInMainCurrency);

            foreach (var product in cart.Items)
            {
                mActivityLogger.LogPurchasedProductActivity(product.OriginalCartItem.SKU, product.Units);
            }

            mActivityLogger.LogPurchaseActivity(orderInfo.OrderID, orderInfo.OrderTotalPriceInMainCurrency, priceString, false);
        }
    }
}
