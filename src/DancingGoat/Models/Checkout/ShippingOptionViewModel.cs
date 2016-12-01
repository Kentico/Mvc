using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using CMS.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "ShippingOptions")]
    public class ShippingOptionViewModel
    {
        [Required(ErrorMessage = "DancingGoatMvc.Shipping.ShippingOptionRequired")]
        [Display(Name = "DancingGoatMvc.Shipping.ShippingOption")]
        public int ShippingOptionID { get; set; }


        public SelectList ShippingOptions { get; set; }


        public ShippingOptionViewModel()
        {
        }


        public ShippingOptionViewModel(ShippingOptionInfo shippingOption, SelectList shippingOptions)
        {
            ShippingOptions = shippingOptions;
            ShippingOptionID = shippingOption?.ShippingOptionID ?? 0;
        }
    }
}