using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using CMS.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "PaymentMethods")]
    public class PaymentMethodViewModel
    {
        [Required(ErrorMessage = "DancingGoatMvc.Payment.PaymentMethodRequired")]
        [Display(Name = "DancingGoatMvc.Payment.SelectPayment")]
        public int PaymentMethodID { get; set; }


        public SelectList PaymentMethods { get; set; }


        public PaymentMethodViewModel()
        {
        }


        public PaymentMethodViewModel(PaymentOptionInfo paymentMethod, SelectList paymentMethods)
        {
            PaymentMethods = paymentMethods;

            if (paymentMethod != null)
            {
                PaymentMethodID = paymentMethod.PaymentOptionID;
            }
        }
    }
}