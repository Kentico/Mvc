using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a customer's order.
    /// </summary>
    public class Order
    {
        private IEnumerable<OrderItem> mOrderItems;
        private OrderAddress mBillingAddress;
        private OrderAddress mShippingAddress;
        private Currency mCurrency;
        private string mStatusName;


        /// <summary>
        /// Original Kentico order info object from which the model gathers information. See <see cref="OrderInfo"/> for detailed information.
        /// </summary>
        internal readonly OrderInfo OriginalOrder;


        /// <summary>
        /// Gets the order's identifier.
        /// </summary>
        public int OrderID => OriginalOrder.OrderID;


        /// <summary>
        /// Gets the order's invoice number.
        /// </summary>
        public string OrderInvoiceNumber => OriginalOrder.OrderInvoiceNumber;


        /// <summary>
        /// Gets the order's creation date.
        /// </summary>
        public DateTime OrderDate => OriginalOrder.OrderDate;


        /// <summary>
        /// Gets the currency used in the order.
        /// </summary>
        public Currency Currency
        {
            get
            {
                if (mCurrency == null)
                {
                    var currency = CurrencyInfoProvider.GetCurrencyInfo(OriginalOrder.OrderCurrencyID);
                    mCurrency = new Currency(currency);
                }

                return mCurrency;
            }
        }


        /// <summary>
        /// Order's billing address. See <see cref="OrderAddress"/> for detailed information.
        /// </summary>
        public OrderAddress BillingAddress
        {
            get
            {
                if ((mBillingAddress == null) && (OriginalOrder.OrderBillingAddress != null))
                {
                    mBillingAddress = new OrderAddress((OrderAddressInfo)OriginalOrder.OrderBillingAddress);
                }

                return mBillingAddress;
            }
        }


        /// <summary>
        /// Order's shipping address. See <see cref="OrderAddress"/> for detailed information.
        /// </summary>
        public OrderAddress ShippingAddress
        {
            get
            {
                if ((mShippingAddress == null) && (OriginalOrder.OrderShippingAddress != null))
                {
                    mShippingAddress = new OrderAddress((OrderAddressInfo)OriginalOrder.OrderShippingAddress);
                }

                return mShippingAddress;
            }
        }


        /// <summary>
        /// Gets the order status's identifier.
        /// </summary>
        public int StatusID => OriginalOrder.OrderStatusID;


        /// <summary>
        /// Order status's display name.
        /// </summary>
        public string StatusName
        {
            get
            {
                return mStatusName ?? (mStatusName =
                           OrderStatusInfoProvider.GetOrderStatusInfo(StatusID)
                                                  ?.StatusDisplayName);
            }
        }


        /// <summary>
        /// Items in the order. See <see cref="OrderItem"/> for detailed information.
        /// </summary>
        public IEnumerable<OrderItem> OrderItems
        {
            get
            {
                return mOrderItems ?? (mOrderItems =
                           OrderItemInfoProvider.GetOrderItems(OrderID)
                                                .Select(orderItem => new OrderItem(orderItem))
                                                .ToList());
            }
        }


        /// <summary>
        /// Indicates if the order is paid.
        /// </summary>
        public bool IsPaid => OriginalOrder.OrderIsPaid;


        /// <summary>
        /// Indicates whether the order payment is authorized.
        /// </summary>
        public bool IsAuthorized => OriginalOrder.OrderIsAuthorized;


        /// <summary>
        /// Gets the order's payment result. See <see cref="PaymentResultInfo"/> for detailed information.
        /// </summary>
        public PaymentResultInfo PaymentResult => OriginalOrder.OrderPaymentResult;


        /// <summary>
        /// Gets the order's total price.
        /// </summary>
        public decimal TotalPrice => OriginalOrder.OrderTotalPrice;


        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="originalOrder"><see cref="PaymentResultInfo"/> object representing an original Kentico order info object from which the model is created.</param>
        public Order(OrderInfo originalOrder)
        {
            if (originalOrder == null)
            {
                throw new ArgumentNullException(nameof(originalOrder));
            }

            OriginalOrder = originalOrder;
        }


        /// <summary>
        /// Marks the order as paid.
        /// </summary>
        public void SetAsPaid()
        {
            if (!IsPaid)
            {
                OriginalOrder.OrderIsPaid = true;
                Save();
            }
        }


        /// <summary>
        /// Sets the payment result to the <see cref="Order"/> object.
        /// </summary>
        /// <remarks>
        /// In case the <see cref="PaymentResultInfo.PaymentIsCompleted"/> property of <paramref name="paymentResult"/> is true, the order is marked as paid and the order status is set according to <see cref="PaymentOptionInfo.PaymentOptionSucceededOrderStatusID"/>.
        /// The <see cref="PaymentResultInfo.PaymentIsFailed"/> property value of <paramref name="paymentResult"/> indicates if the order is marked as unpaid and the order status is set according to <see cref="PaymentOptionInfo.PaymentOptionFailedOrderStatusID"/>.
        /// </remarks>
        /// <param name="paymentResult"><see cref="PaymentResultInfo"/> object representing an original Kentico payment result object from which the model is created.</param>
        public void SetPaymentResult(PaymentResultInfo paymentResult)
        {
            if (paymentResult == null)
            {
                throw new ArgumentNullException(nameof(paymentResult));
            }
            if (paymentResult.PaymentIsCompleted && paymentResult.PaymentIsFailed)
            {
                throw new InvalidOperationException("Order payment failed but paymentResult.PaymentIsCompleted is true");
            }

            OriginalOrder.OrderPaymentResult = paymentResult;

            if (!paymentResult.PaymentIsCompleted && !paymentResult.PaymentIsFailed)
            {
                Save();
                return;
            }

            OriginalOrder.UpdateOrderStatus(paymentResult);
            Save();
        }


        /// <summary>
        /// Checks if order was created by given user.
        /// </summary>
        /// <param name="userID">ID of the user.</param>
        /// <returns><c>true</c> if the order was created by the user.</returns>
        public bool IsCreatedByUser(int userID)
        {
            return OriginalOrder.OrderCompletedByUserID == userID;
        }


        /// <summary>
        /// Saves the order into the database.
        /// </summary>
        private void Save() => OrderInfoProvider.SetOrderInfo(OriginalOrder);
    }
}
