using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "Countries")]
    public class BillingAddressViewModel
    {
        [Required]
        [Display(Name = "DancingGoatMvc.Address.Line1")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressLine1 { get; set; }



        [Display(Name = "DancingGoatMvc.Address.Line2")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressLine2 { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Address.City")]
        [MaxLength(100, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressCity { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Address.Zip")]
        [MaxLength(20, ErrorMessage = "General.MaxlengthExceeded")]
        public string BillingAddressPostalCode { get; set; }
        

        public CountryStateViewModel BillingAddressCountryStateSelector { get; set; }


        public AddressSelectorViewModel BillingAddressSelector { get; set; }


        public SelectList Countries { get; set; }


        public string BillingAddressState { get; set; }


        public string BillingAddressCountry { get; set; }


        public BillingAddressViewModel()
        {
        }


        public BillingAddressViewModel(CustomerAddress address, SelectList countries, SelectList addresses = null)
        {
            if (address != null)
            {
                BillingAddressLine1 = address.Line1;
                BillingAddressLine2 = address.Line2;
                BillingAddressCity = address.City;
                BillingAddressPostalCode = address.PostalCode;
                BillingAddressState = address.State?.StateDisplayName ?? String.Empty;
                BillingAddressCountry = address.Country?.CountryDisplayName ?? String.Empty;
                Countries = countries;
            }

            BillingAddressCountryStateSelector = new CountryStateViewModel
            {
                Countries = countries,
                CountryID = address?.CountryID ?? 0,
                StateID = address?.StateID ?? 0
            };

            BillingAddressSelector = new AddressSelectorViewModel
            {
                Addresses = addresses,
                AddressID = address?.ID ?? 0
            };
        }


        public void ApplyTo(CustomerAddress address)
        {
            address.Line1 = BillingAddressLine1;
            address.Line2 = BillingAddressLine2;
            address.City = BillingAddressCity;
            address.PostalCode = BillingAddressPostalCode;
            address.CountryID = BillingAddressCountryStateSelector.CountryID;
            address.StateID = BillingAddressCountryStateSelector.StateID ?? 0;
        }
    }
}