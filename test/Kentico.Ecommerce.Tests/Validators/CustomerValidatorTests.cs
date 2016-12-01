using NUnit.Framework;

using CMS.Ecommerce;
using CMS.Tests;

namespace Kentico.Ecommerce.Tests
{
    [TestFixture, SharedDatabaseForAllTests]
    class CustomerValidatorTests : IsolatedIntegrationTests
    {
        private Customer mCustomer;
        

        [OneTimeSetUp]
        public void SetUp()
        {
            var customerInfo = new CustomerInfo()
            {
                CustomerFirstName = "Alfred",
                CustomerLastName = "Derfla",
            };

            customerInfo.Insert();

            mCustomer = new Customer(customerInfo);
        }


        [TestCase("invalid@")]
        [TestCase("")]
        public void Validate_InvalidEmail_CheckFailed(string email)
        {
            mCustomer.Email = email;

            var validator = new CustomerValidator(mCustomer);
            validator.Validate();

            CMSAssert.All(
                () => Assert.IsTrue(validator.CheckFailed),
                () => Assert.IsTrue(validator.InvalidEmailFormat)
            );
        }


        [Test]
        public void Validate_ValidEmail_CheckPassed()
        {
            mCustomer.Email = "em@a.il";

            var validator = new CustomerValidator(mCustomer);
            validator.Validate();

            CMSAssert.All(
                () => Assert.IsFalse(validator.CheckFailed),
                () => Assert.IsFalse(validator.InvalidEmailFormat)
            );
        }
    }
}