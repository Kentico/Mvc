using System;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Orders
{
    public class OrdersListViewModel
    {
        public int OrderID { get; set; }


        public string OrderInvoiceNumber { get; set; }


        public DateTime OrderDate { get; set; }


        public string StatusName { get; set; }


        public string FormattedTotalPrice { get; set; }


        public OrdersListViewModel()
        {
        }


        public OrdersListViewModel(Order order)
        {
            if (order == null)
            {
                return;
            }

            OrderID = order.OrderID;
            OrderInvoiceNumber = order.OrderInvoiceNumber;
            OrderDate = order.OrderDate;
            StatusName = order.StatusName;
            FormattedTotalPrice = order.Currency.FormatPrice(order.TotalPrice);
        }
    }
}