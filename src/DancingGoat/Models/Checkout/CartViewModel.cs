using System.Collections.Generic;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class CartViewModel
    {
        public ShoppingCart Cart { get; set; }

        
        public decimal RemainingAmountForFreeShipping { get; set; }


        public IEnumerable<string> AppliedCouponCodes { get; set; }
    }
}