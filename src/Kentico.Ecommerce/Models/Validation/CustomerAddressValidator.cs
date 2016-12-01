using CMS.Globalization;

namespace Kentico.Ecommerce
{
    /// <summary>
    /// Class for customer address validation.
    /// </summary>
    public class CustomerAddressValidator
    {
        private readonly CustomerAddress mAddress;


        /// <summary>
        /// Indicates if some validation failed.
        /// </summary>
        public bool CheckFailed => CountryNotSet || StateNotFromCountry;


        /// <summary>
        /// True when country is not set.
        /// </summary>
        public bool CountryNotSet { get; private set; }


        /// <summary>
        /// True when country does not contain selected state.
        /// </summary>
        public bool StateNotFromCountry { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressValidator"/> class.
        /// </summary>
        /// <param name="address"><see cref="CustomerAddress"/> object representing a customer address that is validated.</param>
        public CustomerAddressValidator(CustomerAddress address)
        {
            mAddress = address;
        }


        /// <summary>
        /// Validates the address.
        /// </summary>
        /// <remarks>
        /// The following conditions must be met to pass the validation:
        /// 1) Country is set.
        /// 2) Country contains selected state.
        /// </remarks>
        public void Validate()
        {
            CountryNotSet = (mAddress.CountryID == 0);

            if (!CountryNotSet)
            {
                var state = StateInfoProvider.GetStateInfo(mAddress.StateID);
                StateNotFromCountry = (state != null) && (state.CountryID != mAddress.CountryID);
            }
        }
    }
}
