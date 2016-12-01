using System;

using CMS.Ecommerce;
using CMS.Globalization;


namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents an address used in an order.
    /// </summary>
    public class OrderAddress 
    {
        private CountryInfo mCountry;
        private StateInfo mState;


        /// <summary>
        /// Original Kentico order address info object from which the model gathers information.
        /// </summary>
        internal OrderAddressInfo OriginalAddress;


        /// <summary>
        /// Address's personal name.
        /// </summary>
        public string PersonalName => OriginalAddress.AddressPersonalName;


        /// <summary>
        /// Address's line 1. 
        /// </summary>
        public string Line1 => OriginalAddress.AddressLine1;


        /// <summary>
        /// Address's line 2. 
        /// </summary>
        public string Line2 => OriginalAddress.AddressLine2;


        /// <summary>
        /// Address's city.
        /// </summary>
        public string City => OriginalAddress.AddressCity;


        /// <summary>
        /// Address's postal code.
        /// </summary>
        public string PostalCode => OriginalAddress.AddressZip;


        /// <summary>
        /// Address's identifier.
        /// </summary>
        public int ID => OriginalAddress.AddressID;


        /// <summary>
        /// Country's identifier.
        /// </summary>
        public int CountryID => Country?.CountryID ?? 0;


        /// <summary>
        /// State's identifier.
        /// </summary>
        public int StateID => State?.StateID ?? 0;


        /// <summary>
        /// Address's country. See <see cref="CountryInfo"/> for detailed information.
        /// </summary>
        public CountryInfo Country
        {
            get
            {
                if ((mCountry == null) || (mCountry.CountryID != OriginalAddress.AddressCountryID))
                {
                    mCountry = CountryInfoProvider.GetCountryInfo(OriginalAddress.AddressCountryID);
                }

                return mCountry;
            }
        }


        /// <summary>
        /// Address's state. See <see cref="StateInfo"/> for detailed information.
        /// </summary>
        public StateInfo State
        {
            get
            {
                if ((mState == null) || (mState.StateID != OriginalAddress.AddressStateID))
                {
                    mState = StateInfoProvider.GetStateInfo(OriginalAddress.AddressStateID);
                }

                return mState;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAddress"/> class.
        /// </summary>
        /// <param name="originalAddress"><see cref="OrderAddressInfo"/> object representing an original Kentico order address info object from which the model is created.</param>
        public OrderAddress(OrderAddressInfo originalAddress)
        {
            if (originalAddress == null)
            {
                throw new ArgumentNullException(nameof(originalAddress));
            }

            OriginalAddress = originalAddress;
        }        
    }
}
