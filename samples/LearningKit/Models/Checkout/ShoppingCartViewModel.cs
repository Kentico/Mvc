using System.Collections.Generic;

using Kentico.Ecommerce;

namespace LearningKit.Models.Checkout
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart Cart { get; set; }
        public IEnumerable<ShoppingCartItem> Items => Cart.Items;
        public string CouponCode { get; set; }
        public decimal RemainingAmountForFreeShipping { get; set; }
    }
}