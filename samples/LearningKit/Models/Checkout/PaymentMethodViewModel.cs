using System.Web.Mvc;

using CMS.Ecommerce;

namespace LearningKit.Models.Checkout
{
    //DocSection:PaymentViewModel
    public class PaymentMethodViewModel
    {
        public int PaymentMethodID { get; set; }
        public SelectList PaymentMethods { get; set; }
        
        /// <summary>
        /// Creates a payment method model.
        /// </summary>
        /// <param name="paymentMethod">Selected payment method.</param>
        /// <param name="paymentMethods">List of all available payment methods.</param>
        public PaymentMethodViewModel(PaymentOptionInfo paymentMethod, SelectList paymentMethods)
        {
            PaymentMethods = paymentMethods;
            
            if (paymentMethod != null)
            {
                PaymentMethodID = paymentMethod.PaymentOptionID;
            }
        }
        
        /// <summary>
        /// Creates an empty payment method model.
        /// </summary>
        public PaymentMethodViewModel()
        {
        }
    }
    //EndDocSection:PaymentViewModel
}