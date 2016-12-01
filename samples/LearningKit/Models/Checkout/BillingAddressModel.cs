using System.Web.Mvc;

using Kentico.Ecommerce;

namespace LearningKit.Models.Checkout
{
    //DocSection:BillingAddressModel
    public class BillingAddressModel
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int AddressID { get; set; }
        public SelectList Countries { get; set; }
        public SelectList Addresses { get; set; }
        
        /// <summary>
        /// Creates a billing address model.
        /// </summary>
        /// <param name="address">Billing address.</param>
        /// <param name="countryList">List of countries.</param>
        public BillingAddressModel(CustomerAddress address, SelectList countries, SelectList addresses)
        {
            if (address != null)
            {
                Line1 = address.Line1;
                Line2 = address.Line2;
                City = address.City;
                PostalCode = address.PostalCode;
                CountryID = address.CountryID;
                StateID = address.StateID;
                AddressID = address.ID;
            }
            
            Countries = countries;
            Addresses = addresses;
        }
        
        /// <summary>
        /// Creates an empty billing address model.
        /// </summary>
        public BillingAddressModel()
        {
        }
        
        /// <summary>
        /// Applies the model to an address wrapper.
        /// </summary>
        /// <param name="address">Billing address to which the model is applied.</param>
        public void ApplyTo(CustomerAddress address)
        {
            address.Line1 = Line1;
            address.Line2 = Line2;
            address.City = City;
            address.PostalCode = PostalCode;
            address.CountryID = CountryID;
            address.StateID = StateID;
        }
    }
    //EndDocSection:BillingAddressModel
}