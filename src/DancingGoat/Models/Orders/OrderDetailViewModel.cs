using System.Collections.Generic;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Orders
{
    public class OrderDetailViewModel
    {
        public string InvoiceNumber { get; set; }


        public decimal TotalPrice { get; set; }


        public string StatusName { get; set; }


        public OrderAddressViewModel OrderAddress { get; set; }


        public IEnumerable<OrderItem> OrderItems { get; set; }


        public Currency OrderCurrency { get; set; }
    }
}