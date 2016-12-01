using Kentico.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class PreviewViewModel
    {
        public DeliveryDetailsViewModel DeliveryDetails { get; set; }

        
        public CartViewModel CartModel { get; set; }


        public CustomerViewModel CustomerDetails => DeliveryDetails.Customer;


        public BillingAddressViewModel BillingAddress => DeliveryDetails.BillingAddress;
        

        public PaymentMethodViewModel PaymentMethod { get;  set; }


        public ShoppingCart Cart => CartModel.Cart;


        public string ShippingName { get; set; }
    }
}