using System.ComponentModel.DataAnnotations;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class CartViewModel
    {
        public ShoppingCart Cart { get; set; }

        
        public decimal RemainingAmountForFreeShipping { get; set; }


        [Display(Name = "DancingGoatMvc.Checkout.CouponCode")]
        [MaxLength(200, ErrorMessage = "General.MaxlengthExceeded")]
        public string CouponCode { get; set; }
    }
}