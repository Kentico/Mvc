using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;
using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Ecommerce.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class KenticoShippingOptionRepositoryTests : UnitTests
    {
        private KenticoShippingOptionRepository mRepository;

        private const int SITE_ID1 = 1;
        private const int SITE_ID2 = 2;

        private const int SHIPPINGOPTION_CARRIER_ID = 1;

        private const int ENABLED_SHIPPINGOPTION_ID1 = 1;
        private const int ENABLED_SHIPPINGOPTION_ID2 = 2;
        private const int DISABLED_SHIPPINGOPTION_ID = 3;
        private const int CROSSSITE_SHIPPINGOPTION_ID = 4;
        private const int NON_EXISTING_SHIPPINGOPTION_ID = 5;
        

        [SetUp]
        public void SetUp()
        {
            SetUpSite();
            SetUpShippingOptions();

            SiteContext.CurrentSite = SiteInfoProvider.GetSiteInfo(SITE_ID1);

            mRepository = new KenticoShippingOptionRepository();
        }


        [Test]
        public void GetShippingOption_EnabledShippingOption_ReturnsShippingOption()
        {
            var shippingOption = mRepository.GetById(ENABLED_SHIPPINGOPTION_ID1);

            CMSAssert.All(
                () => Assert.IsNotNull(shippingOption, "Returned null for an existing enabled shipping option."),
                () => Assert.AreEqual(ENABLED_SHIPPINGOPTION_ID1, shippingOption.ShippingOptionID, "Incorrect shipping option returned.")
            );
        }


        [Test]
        public void GetShippingOption_DisabledShippingOption_ReturnsShippingOption()
        {
            var shippingOption = mRepository.GetById(DISABLED_SHIPPINGOPTION_ID);

            CMSAssert.All(
                () => Assert.IsNotNull(shippingOption, "Returned null for an existing disabled shipping option."),
                () => Assert.AreEqual(DISABLED_SHIPPINGOPTION_ID, shippingOption.ShippingOptionID, "Incorrect shipping option returned.")
            );
        }


        [Test]
        public void GetShippingOption_CrossSiteShippingOption_ReturnsNull()
        {
            var shippingOption = mRepository.GetById(CROSSSITE_SHIPPINGOPTION_ID);

            Assert.IsNull(shippingOption, "Object returned for cross-site shipping option request.");
        }


        [Test]
        public void GetShippingOption_NonExistingShippingOption_ReturnsNull()
        {
            var shippingOption = mRepository.GetById(NON_EXISTING_SHIPPINGOPTION_ID);

            Assert.IsNull(shippingOption, "Object returned for non-existing shipping option.");
        }


        [Test]
        public void GetShippingOptions_ReturnsAllEnabledOptions()
        {
            var shippingOptions = mRepository.GetAllEnabled();

            CMSAssert.All(
                () => Assert.IsNotNull(shippingOptions, "No shipping option returned."),
                () => Assert.IsNotEmpty(shippingOptions, "No enabled shipping options found."),
                () => Assert.AreEqual(2, shippingOptions.Count(), "Wrong number of shipping options returned.")
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


        private void SetUpShippingOptions()
        {
            Fake<CarrierInfo, CarrierInfoProvider>().WithData(
                new CarrierInfo
                {
                    CarrierID = SHIPPINGOPTION_CARRIER_ID,
                    CarrierSiteID = SITE_ID1
                });

            Fake<ShippingOptionInfo, ShippingOptionInfoProvider>().WithData(
                new ShippingOptionInfo
                {
                    ShippingOptionCarrierID = SHIPPINGOPTION_CARRIER_ID,
                    ShippingOptionID = ENABLED_SHIPPINGOPTION_ID1,
                    ShippingOptionSiteID = SITE_ID1,
                    ShippingOptionEnabled = true
                },
                new ShippingOptionInfo
                {
                    ShippingOptionCarrierID = SHIPPINGOPTION_CARRIER_ID,
                    ShippingOptionID = ENABLED_SHIPPINGOPTION_ID2,
                    ShippingOptionSiteID = SITE_ID1,
                    ShippingOptionEnabled = true
                },
                new ShippingOptionInfo
                {
                    ShippingOptionCarrierID = SHIPPINGOPTION_CARRIER_ID,
                    ShippingOptionID = DISABLED_SHIPPINGOPTION_ID,
                    ShippingOptionSiteID = SITE_ID1,
                    ShippingOptionEnabled = false
                },
                new ShippingOptionInfo
                {
                    ShippingOptionCarrierID = SHIPPINGOPTION_CARRIER_ID,
                    ShippingOptionID = CROSSSITE_SHIPPINGOPTION_ID,
                    ShippingOptionSiteID = SITE_ID2,
                    ShippingOptionEnabled = true
                });
        }
    }
}
