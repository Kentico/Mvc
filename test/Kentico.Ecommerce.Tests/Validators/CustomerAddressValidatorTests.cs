using CMS.Globalization;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    class CustomerAddressValidatorTests : EcommerceTestsBase
    {
        private StateInfo mStateFromUnknownCountry;


        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            SetUpSite();

            var unknownCountry = new CountryInfo()
            {
                CountryDisplayName = "Unknown",
                CountryName = "Unknown",
                CountryThreeLetterCode = "UKW"
            };
            unknownCountry.Insert();

            mStateFromUnknownCountry = new StateInfo()
            {
                StateDisplayName = "State NFC",
                StateName = "StateNFC",
                CountryID = unknownCountry.CountryID
            };
            mStateFromUnknownCountry.Insert();
        }


        [SetUp]
        public void SetUp()
        {
            // Reset address settings
            Factory.CustomerAddressUSA.AddressCountryID = Factory.CountryUSA.CountryID;
            Factory.CustomerAddressUSA.AddressStateID = Factory.StateWashington.StateID;
        }


        [Test]
        public void Validate_CountryNotSet_CheckFailed()
        {
            var address = new CustomerAddress(Factory.CustomerAddressUSA);
            address.Country = null;

            var validator = new CustomerAddressValidator(address);
            validator.Validate();

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.CountryNotSet)
            );
        }


        [Test]
        public void Validate_StateNotFromCountry_CheckFailed()
        {
            var address = new CustomerAddress(Factory.CustomerAddressUSA);
            address.State = mStateFromUnknownCountry;

            var validator = new CustomerAddressValidator(address);
            validator.Validate();

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.StateNotFromCountry)
            );
        }


        [Test]
        public void Validate_ValidCountry_CheckPassed()
        {
            var address = new CustomerAddress(Factory.CustomerAddressUSA);

            var validator = new CustomerAddressValidator(address);
            validator.Validate();

            CMSAssert.All(
                () => Assert.IsFalse(validator.CheckFailed),
                () => Assert.IsFalse(validator.CountryNotSet),
                () => Assert.IsFalse(validator.StateNotFromCountry)
            );
        }
    }
}
