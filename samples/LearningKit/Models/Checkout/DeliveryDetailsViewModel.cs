namespace LearningKit.Models.Checkout
{
    public class DeliveryDetailsViewModel
    {
        public CustomerModel Customer { get; set; }
        public BillingAddressModel BillingAddress { get; set; }
        public ShippingOptionModel ShippingOption { get; set; }
    }
}