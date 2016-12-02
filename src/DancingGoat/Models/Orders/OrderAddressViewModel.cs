using Kentico.Ecommerce;

namespace DancingGoat.Models.Orders
{
    public class OrderAddressViewModel
    {
        public string AddressLine1 { get; set; }


        public string AddressLine2 { get; set; }


        public string AddressCity { get; set; }


        public string AddressPostalCode { get; set; }


        public string AddressState { get; set; }


        public string AddressCountry { get; set; }


        public OrderAddressViewModel()
        {
        }


        public OrderAddressViewModel(OrderAddress address)
        {
            if (address == null)
            {
                return;
            }

            AddressLine1 = address.Line1;
            AddressLine2 = address.Line2;
            AddressCity = address.City;
            AddressPostalCode = address.PostalCode;
            AddressState = address.State?.StateDisplayName ?? string.Empty;
            AddressCountry = address.Country?.CountryDisplayName ?? string.Empty;
        }
    }
}