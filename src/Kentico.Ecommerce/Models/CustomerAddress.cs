using System;

using CMS.Ecommerce;
using CMS.Globalization;
using CMS.Helpers;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Represents a customer's address.
    /// </summary>
    public class CustomerAddress
    {
        private CountryInfo mCountry;
        private StateInfo mState;


        /// <summary>
        /// Original Kentico address info object from which the model gathers information.
        /// </summary>
        internal AddressInfo OriginalAddress;


        /// <summary>
        /// Address's personal name.
        /// </summary>
        public string PersonalName
        {
            get
            {
                return OriginalAddress.AddressPersonalName;
            }
            set
            {
                OriginalAddress.AddressPersonalName = TextHelper.LimitLength(value, 200);
            }
        }


        /// <summary>
        /// Address's line 1. 
        /// </summary>
        public string Line1
        {
            get
            {
                return OriginalAddress.AddressLine1;
            }
            set
            {
                OriginalAddress.AddressLine1 = value;
            }
        }


        /// <summary>
        /// Address's line 2. 
        /// </summary>
        public string Line2
        {
            get
            {
                return OriginalAddress.AddressLine2;
            }
            set
            {
                OriginalAddress.AddressLine2 = value;
            }
        }


        /// <summary>
        /// Address's city.
        /// </summary>
        public string City
        {
            get
            {
                return OriginalAddress.AddressCity;
            }
            set
            {
                OriginalAddress.AddressCity = value;
            }
        }


        /// <summary>
        /// Address's postal code.
        /// </summary>
        public string PostalCode
        {
            get
            {
                return OriginalAddress.AddressZip;
            }
            set
            {
                OriginalAddress.AddressZip = value;
            }
        }


        /// <summary>
        /// Address's identifier.
        /// </summary>
        public int ID => OriginalAddress.AddressID;


        /// <summary>
        /// Country's identifier.
        /// </summary>
        public int CountryID
        {
            get
            {
                return Country?.CountryID ?? 0;
            }
            set
            {
                Country = CountryInfoProvider.GetCountryInfo(value);
            }
        }


        /// <summary>
        /// State's identifier.
        /// </summary>
        public int StateID
        {
            get
            {
                return State?.StateID ?? 0;
            }
            set
            {
                State = StateInfoProvider.GetStateInfo(value);
            }
        }


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
            set
            {
                OriginalAddress.AddressCountryID = value?.CountryID ?? 0;
                mCountry = null;
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
            set
            {
                OriginalAddress.AddressStateID = value?.StateID ?? 0;
                mState = null;
            }
        }


        /// <summary>
        /// Gets an address name created from the address parameters. The format is [personal or company name], [address line 1], [address line 2], [city].
        /// </summary>
        public string Name => OriginalAddress.AddressName;


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddress"/> class.
        /// </summary>
        public CustomerAddress()
            : this(new AddressInfo())
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddress"/> class.
        /// </summary>
        /// <param name="originalAddress"><see cref="AddressInfo"/> object representing an original Kentico address info object from which the model is created.</param>
        public CustomerAddress(AddressInfo originalAddress)
        {
            if (originalAddress == null)
            {
                throw new ArgumentNullException(nameof(originalAddress));
            }

            OriginalAddress = originalAddress;
        }


        /// <summary>
        /// Validates the customer address.
        /// </summary>
        /// <returns>Instance of the <see cref="CustomerAddressValidator"/> with validation summary.</returns>
        public CustomerAddressValidator Validate()
        {
            var validator = new CustomerAddressValidator(this);
            validator.Validate();
            return validator;
        }


        /// <summary>
        /// Saves the address into the database.
        /// </summary>
        /// <param name="validate">Specifies whether the validation should be performed.</param>
        public void Save(bool validate = true)
        {
            if (validate)
            {
                var result = Validate();
                if (result.CheckFailed)
                {
                    throw new InvalidOperationException();
                }
            }

            OriginalAddress.AddressName = AddressInfoProvider.GetAddressName(OriginalAddress);
            AddressInfoProvider.SetAddressInfo(OriginalAddress);
        }
    }
}
