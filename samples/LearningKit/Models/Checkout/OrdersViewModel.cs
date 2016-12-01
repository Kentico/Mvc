using System.Collections.Generic;

using Kentico.Ecommerce;

namespace LearningKit.Models.Checkout
{
    public class OrdersViewModel
    {
        public IEnumerable<Order> Orders
        {
            get;
            set;
        }
    }
}
