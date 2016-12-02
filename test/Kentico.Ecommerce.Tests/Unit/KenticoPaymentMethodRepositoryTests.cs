using System.Linq;

using CMS.Ecommerce;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class KenticoPaymentMethodRepositoryTests : UnitTests
    {
        private KenticoPaymentMethodRepository mRepository;

        private const int SITE_ID1 = 1;
        private const int SITE_ID2 = 2;

        private const int ENABLED_PAYMENTMETHOD_ID1 = 1;
        private const int ENABLED_PAYMENTMETHOD_ID2 = 2;
        private const int DISABLED_PAYMENTMETHOD_ID = 3;
        private const int GLOBAL_PAYMENTMETHOD_ID = 4;
        private const int CROSSSITE_PAYMENTMETHOD_ID = 5;
        private const int NON_EXISTING_PAYMENTMETHOD_ID = 6;


        [SetUp]
        public void SetUp()
        {
            SetUpSite();
            SetUpPaymentMethods();

            SiteContext.CurrentSite = SiteInfoProvider.GetSiteInfo(SITE_ID1);

            mRepository = new KenticoPaymentMethodRepository();
        }


        [Test]
        public void GetPaymentMethod_EnabledPaymentMethod_ReturnsPaymentMethod()
        {
            var paymentMethod = mRepository.GetById(ENABLED_PAYMENTMETHOD_ID1);

            CMSAssert.All(
                () => Assert.IsNotNull(paymentMethod, "Returned null for an existing enabled payment option."),
                () => Assert.AreEqual(ENABLED_PAYMENTMETHOD_ID1, paymentMethod.PaymentOptionID, "Incorrect payment option returned.")
            );
        }


        [Test]
        public void GetPaymentMethod_DisabledPaymentMethod_ReturnsPaymentMethod()
        {
            var paymentMethod = mRepository.GetById(DISABLED_PAYMENTMETHOD_ID);

            CMSAssert.All(
                () => Assert.IsNotNull(paymentMethod, "Returned null for an existing disabled payment option."),
                () => Assert.AreEqual(DISABLED_PAYMENTMETHOD_ID, paymentMethod.PaymentOptionID, "Incorrect payment option returned.")
            );
        }


        [Test]
        public void GetPaymentMethod_NonExistingPaymentMethod_ReturnsNull()
        {
            var paymentMethod = mRepository.GetById(NON_EXISTING_PAYMENTMETHOD_ID);

            Assert.IsNull(paymentMethod, "Object returned for non-existing payment option.");
        }


        [Test]
        public void GetPaymentMethod_GlobalPaymentMethod_GlobalEnabled_ReturnsPaymentMethod()
        {
            EnableGlobalMethods(true);
            var paymentMethod = mRepository.GetById(GLOBAL_PAYMENTMETHOD_ID);

            CMSAssert.All(
                () => Assert.IsNotNull(paymentMethod, "Returned null for an existing enabled global payment option."),
                () => Assert.AreEqual(GLOBAL_PAYMENTMETHOD_ID, paymentMethod.PaymentOptionID, "Incorrect payment option returned.")
            );
        }

        [Test]
        public void GetPaymentMethod_GlobalPaymentMethod_GlobalDisabled_ReturnsPaymentMethod()
        {
            EnableGlobalMethods(false);
            var paymentMethod = mRepository.GetById(GLOBAL_PAYMENTMETHOD_ID);

            Assert.IsNull(paymentMethod, "Returned global payment method, when global methods are disabled.");
        }


        [Test]
        public void GetPaymentMethod_CrossSitePaymentMethod_ReturnsNull()
        {
            var paymentMethod = mRepository.GetById(CROSSSITE_PAYMENTMETHOD_ID);

            Assert.IsNull(paymentMethod, "Object returned for cross-site payment option request.");
        }


        [Test]
        public void GetPaymentMethods_GlobalEnabled_ReturnsAllEnabledOptions()
        {
            EnableGlobalMethods(true);
            var paymentMethods = mRepository.GetAll();

            CMSAssert.All(
                () => Assert.IsNotNull(paymentMethods, "No payment option returned."),
                () => Assert.IsNotEmpty(paymentMethods, "No enabled payment options found."),
                () => Assert.AreEqual(3, paymentMethods.Count(), "Wrong number of payment options returned.")
            );
        }


        [Test]
        public void GetPaymentMethods_GlobalDisabled_ReturnsAllEnabledOptions()
        {
            EnableGlobalMethods(false);
            var paymentMethods = mRepository.GetAll();

            CMSAssert.All(
                () => Assert.IsNotNull(paymentMethods, "No payment option returned."),
                () => Assert.IsNotEmpty(paymentMethods, "No enabled payment options found."),
                () => Assert.AreEqual(2, paymentMethods.Count(), "Wrong number of payment options returned.")
            );
        }


        private void SetUpSite()
        {
            Fake<SiteInfo, SiteInfoProvider>().WithData(
                new SiteInfo
                {
                    SiteName = "testSite",
                    SiteID = SITE_ID1
                });
        }


        private void SetUpPaymentMethods()
        {
            Fake<PaymentOptionInfo, PaymentOptionInfoProvider>().WithData(
                new PaymentOptionInfo
                {
                    PaymentOptionID = ENABLED_PAYMENTMETHOD_ID1,
                    PaymentOptionSiteID = SITE_ID1,
                    PaymentOptionEnabled = true
                },
                new PaymentOptionInfo
                {
                    PaymentOptionID = ENABLED_PAYMENTMETHOD_ID2,
                    PaymentOptionSiteID = SITE_ID1,
                    PaymentOptionEnabled = true
                },
                new PaymentOptionInfo
                {
                    PaymentOptionID = DISABLED_PAYMENTMETHOD_ID,
                    PaymentOptionSiteID = SITE_ID1,
                    PaymentOptionEnabled = false
                },
                new PaymentOptionInfo
                {
                    PaymentOptionID = GLOBAL_PAYMENTMETHOD_ID,
                    PaymentOptionSiteID = 0,
                    PaymentOptionEnabled = true
                },
                new PaymentOptionInfo
                {
                    PaymentOptionID = CROSSSITE_PAYMENTMETHOD_ID,
                    PaymentOptionSiteID = SITE_ID2,
                    PaymentOptionEnabled = true
                });
        }


        private void EnableGlobalMethods(bool value)
        {
            Fake<SettingsKeyInfo, SettingsKeyInfoProvider>().WithData(
                    new SettingsKeyInfo
                    {
                        SiteID = SITE_ID1,
                        KeyName = ECommerceSettings.ALLOW_GLOBAL_PAYMENT_METHODS,
                        KeyValue = value.ToString()
                    });
        }
    }
}
