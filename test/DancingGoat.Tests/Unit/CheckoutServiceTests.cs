using System.Collections.Generic;

using CMS.Tests;
using CMS.Ecommerce;
using CMS.Globalization;

using DancingGoat.Repositories;
using DancingGoat.Services;
using Kentico.Ecommerce;

using NSubstitute;
using NUnit.Framework;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    public class CheckoutServiceTests : UnitTests
    {
        private CheckoutService checkoutService;

        private const int SHIPPING_OPTION_ID = 33;
        private const int SHIPPING_OPTION_NOT_EXISTING_ID = 17;
        private const int PAYMENT_METHOD_ID = 33;
        private const int PAYMENT_METHOD_NOT_APPLICABLE_ID = 34;
        private const int PAYMENT_METHOD_NOT_EXISTING_ID = 17;
        private const int COUNTRY_ID = 12;
        private const int COUNTRY_NOT_EXISTING_ID = 66;
        private const int COUNTRY_WITHOUT_STATES_ID = 4444;
        private const int STATE_ID = 8;


        [SetUp]
        public void SetUp()
        {
            Fake<ShippingOptionInfo, ShippingOptionInfoProvider>();
            Fake<PaymentOptionInfo, PaymentOptionInfoProvider>();
            Fake<ShoppingCartInfo, ShoppingCartInfoProvider>();
            Fake<CountryInfo, CountryInfoProvider>();
            Fake<StateInfo, StateInfoProvider>();

            var cart = new ShoppingCart(new ShoppingCartInfo());

            var paymentMethodRepository = Substitute.For<IPaymentMethodRepository>();
            var shippingOptionRepository = Substitute.For<IShippingOptionRepository>();
            var countryRepository = Substitute.For<ICountryRepository>();
            var addressRepository = Substitute.For<ICustomerAddressRepository>();
            var shoppingService = Substitute.For<ShoppingService>();
            var pricingService = Substitute.For<PricingService>();
            shoppingService.GetCurrentShoppingCart().Returns(cart);

            var shippingUSPS = new ShippingOptionInfo()
            {
                ShippingOptionID = SHIPPING_OPTION_ID,
                ShippingOptionName = "ShippingUSPS"
            };

            var paymentCashOnDelivery = new PaymentOptionInfo
            {
                PaymentOptionID = PAYMENT_METHOD_ID,
                PaymentOptionName = "CashOnDelivery"
            };

            var paymentNotApplicable = new PaymentOptionInfo
            {
                PaymentOptionID = PAYMENT_METHOD_NOT_APPLICABLE_ID,
                PaymentOptionName = "PaymentNotApplicable"
            };

            // Override method IsPaymentOptionApplicableInternal to make payment method not applicable
            PaymentOptionInfoProvider.ProviderObject = new LocalPaymentOptionInfoProvider();

            var countryUSA = new CountryInfo
            {
                CountryName = "USA",
                CountryID = COUNTRY_ID
            };

            var countryWithoutStates = new CountryInfo
            {
                CountryName = "Canada",
                CountryID = COUNTRY_WITHOUT_STATES_ID
            };

            var stateOregon = new StateInfo
            {
                StateName = "Oregon",
                StateID = STATE_ID,
                CountryID = COUNTRY_ID
            };
            
            shippingOptionRepository.GetAllEnabled().Returns(new List<ShippingOptionInfo> { shippingUSPS });
            paymentMethodRepository.GetAll().Returns(new List<PaymentOptionInfo> { paymentCashOnDelivery, paymentNotApplicable });
            countryRepository.GetCountryStates(COUNTRY_ID).Returns(new List<StateInfo> { stateOregon });
            countryRepository.GetCountryStates(COUNTRY_WITHOUT_STATES_ID).Returns(new List<StateInfo> { });
            countryRepository.GetCountry(COUNTRY_ID).Returns(countryUSA);

            checkoutService = new CheckoutService(shoppingService, pricingService, addressRepository, paymentMethodRepository, shippingOptionRepository, countryRepository);
        }


        [Test]
        public void IsCouponCodeValueValid_EmptyCouponIsValid()
        {
            Assert.IsTrue(checkoutService.IsCouponCodeValueValid(string.Empty), "Empty coupon is valid");
        }


        [Test]
        public void IsCouponCodeValueValid_NullCouponIsValid()
        {
            Assert.IsTrue(checkoutService.IsCouponCodeValueValid(null), "Empty coupon is valid");
        }


        [Test]
        public void IsCouponCodeValueValid_False_For_InvalidCouponCode()
        {
            Assert.IsFalse(checkoutService.IsCouponCodeValueValid("not_valid"), "Coupon code not accepted by shopping cart is invalid.");
        }


        [Test]
        public void IsShippingOptionValid_TrueForValidOption()
        {
            Assert.IsTrue(checkoutService.IsShippingOptionValid(SHIPPING_OPTION_ID), "Existing shipping option is valid");
        }


        [Test]
        public void IsShippingOptionValid_ReturnsFalseIfNotExists()
        {
            Assert.IsFalse(checkoutService.IsShippingOptionValid(SHIPPING_OPTION_NOT_EXISTING_ID), "Option should be invalid");
        }


        [Test]
        public void IsPaymentMethodValid_TrueForValidMethod()
        {
            Assert.IsTrue(checkoutService.IsPaymentMethodValid(PAYMENT_METHOD_ID), "Existing payment method is valid");
        }


        [Test]
        public void IsPaymentMethodValid_FalseForNotApplicable()
        {
            Assert.IsFalse(checkoutService.IsPaymentMethodValid(PAYMENT_METHOD_NOT_APPLICABLE_ID), "Existing payment method is not applicable");
        }


        [Test]
        public void IsPaymentMethodValid_ReturnsFalseIfNotExists()
        {
            Assert.IsFalse(checkoutService.IsPaymentMethodValid(PAYMENT_METHOD_NOT_EXISTING_ID), "Method should be invalid.");
        }


        [Test]
        public void IsStateValid_Valid_For_Existing_Country_State_Pair()
        {
            Assert.IsTrue(checkoutService.IsStateValid(COUNTRY_ID, STATE_ID), "State should be valid");
        }


        [Test]
        public void IsStateValid_Valid_For_Country_Without_States()
        {
            Assert.IsTrue(checkoutService.IsStateValid(COUNTRY_WITHOUT_STATES_ID, null), "Should return true if state is not defined and country does not have states.");
        }


        [Test]
        public void IsStateValid_Invalid_For_Not_Existing_Contry_State_Pair()
        {
            Assert.IsTrue(checkoutService.IsStateValid(COUNTRY_WITHOUT_STATES_ID, STATE_ID), "State is invalid for the country");
        }


        [Test]
        public void IsCountryValid_Existing_Country_Is_Valid()
        {
            Assert.IsTrue(checkoutService.IsCountryValid(COUNTRY_ID), "Existing country is valid.");
        }

        
        [Test]
        public void IsCountryValid_False_For_Not_Existing_Country()
        {
            Assert.IsFalse(checkoutService.IsCountryValid(COUNTRY_NOT_EXISTING_ID), "Country is invalid.");
        }


        private class LocalPaymentOptionInfoProvider : PaymentOptionInfoProvider
        {
            protected override bool IsPaymentOptionApplicableInternal(ShoppingCartInfo cart, PaymentOptionInfo paymentOption)
            {
                return paymentOption.PaymentOptionID != PAYMENT_METHOD_NOT_APPLICABLE_ID;
            }
        }
    }
}
