using System.Threading.Tasks;
using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers.Internal;
using CMS.Membership;
using CMS.Tests;


using NUnit.Framework;

namespace Kentico.ContactManagement.Tests
{
    [TestFixture]
    [Category.Unit]
    public class ContactTrackingServiceTests : UnitTests
    {
        private CurrentContactProviderFake mCurrectContactProvider;
        private IContactTrackingService mContactTrackingService;

        [SetUp]
        public void SetUp()
        { 
            Service<ICurrentContactProvider>.Use<CurrentContactProviderFake>();
            Service<ISiteService>.Use<SiteServiceFake>();
            Service<ILicenseService>.Use<LicenseServiceFake>();
            Service<ICrawlerChecker>.Use<CrawlerCheckerFake>();
            Service<ISettingsService>.Use<SettingsServiceFake>();

            mContactTrackingService = new ContactTrackingService();
            mCurrectContactProvider = Service<ICurrentContactProvider>.Entry() as CurrentContactProviderFake;

            Fake<ContactInfo>();
            Fake<UserInfo, UserInfoProvider>().WithData(
                new UserInfo
                {
                    UserName = "public",
                },
                new UserInfo
                {
                    UserName = "testName",
                    SiteIndependentPrivilegeLevel = UserPrivilegeLevelEnum.Admin,
                    UserID = 5
                });
        }


        [Test]
        public async Task GetCurrentContactAsync_IsCrawler_ReturnsNull()
        {
            // Arrange
            var crawlerService = (CrawlerCheckerFake)Service<ICrawlerChecker>.Entry();
            crawlerService.Crawler = true;

            // Act
            var result = await mContactTrackingService.GetCurrentContactAsync(string.Empty);

            // Assert
            CMSAssert.All(
                () => Assert.That(result, Is.Null, "Result contact should be null"),
                () => Assert.That(() => mCurrectContactProvider.GetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.GetCurrentContact should not be called."),
                () => Assert.That(() => mCurrectContactProvider.SetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.SetCurrentContact should not be called.")
            );
        }


        [Test]
        public async Task GetCurrentContactAsync_InvalidLicense_ReturnsNull()
        {
            // Arrange
            var licenceService = (LicenseServiceFake)Service<ILicenseService>.Entry();
            licenceService.HasLicence = false;

            // Act
            var result = await mContactTrackingService.GetCurrentContactAsync(string.Empty);

            // Assert
            CMSAssert.All(
                () => Assert.That(result, Is.Null, "Result contact should be null"),
                () => Assert.That(() => mCurrectContactProvider.GetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.GetCurrentContact should not be called."),
                () => Assert.That(() => mCurrectContactProvider.SetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.SetCurrentContact should not be called.")
            );
        }


        [TestCase("", "public", Description = "Non existing user")]
        [TestCase("testName", "testName", Description = "Existing user")]
        public async Task GetCurrentContactAsync_CorrectEnvironment_GetsAndSetsContact(string userName, string expectedCallingUserName)
        {
            // Arrange
            var expectedContact = new ContactInfo
            {
                ContactLastName = "TestContact"
            };
            mCurrectContactProvider.CurrentContact = expectedContact;

            // Act
            var result = await mContactTrackingService.GetCurrentContactAsync(userName);

            // Assert
            CMSAssert.All(
                () => Assert.That(result, Is.SameAs(expectedContact), "Result contact should be the same as the expected one"),
                () => Assert.That(() => mCurrectContactProvider.GetCurrentContactFlag, Is.EqualTo(1), "CurrectContactProvider.GetCurrentContact should be called."),
                () => Assert.That(() => mCurrectContactProvider.SetCurrentContactFlag, Is.EqualTo(1), "CurrectContactProvider.SetCurrentContact should be called.")
            );
        }


        [Test]
        public async Task MergeUserContactsAsync_IsCrawler_ReturnsNull()
        {
            // Arrange
            var crawlerService = (CrawlerCheckerFake)Service<ICrawlerChecker>.Entry();
            crawlerService.Crawler = true;

            // Act
            await mContactTrackingService.MergeUserContactsAsync(string.Empty);

            // Assert
            CMSAssert.All(
                () => Assert.That(() => mCurrectContactProvider.GetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.GetCurrentContact should not be called."),
                () => Assert.That(() => mCurrectContactProvider.SetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.SetCurrentContact should not be called.")
            );
        }


        [Test]
        public async Task MergeUserContactsAsync_InvalidLicense_ReturnsNull()
        {
            // Arrange
            var licenceService = (LicenseServiceFake)Service<ILicenseService>.Entry();
            licenceService.HasLicence = false;

            // Act
            await mContactTrackingService.MergeUserContactsAsync(string.Empty);

            // Assert
            CMSAssert.All(
                () => Assert.That(() => mCurrectContactProvider.GetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.GetCurrentContact should not be called."),
                () => Assert.That(() => mCurrectContactProvider.SetCurrentContactFlag, Is.EqualTo(0), "CurrectContactProvider.SetCurrentContact should not be called.")
            );
        }


        [TestCase("", "public", Description = "Non existing user")]
        [TestCase("testName", "testName", Description = "Existing user")]
        public async Task MergeUserContactsAsync_CorrectEnvironment_GetsAndSetsContact(string userName, string expectedCallingUserName)
        {
            // Arrange
            var expectedContact = new ContactInfo
            {
                ContactLastName = "TestContact"
            };
            mCurrectContactProvider.CurrentContact = expectedContact;

            // Act
            await mContactTrackingService.MergeUserContactsAsync(userName);

            // Assert
            CMSAssert.All(
                () => Assert.That(() => mCurrectContactProvider.GetCurrentContactFlag, Is.EqualTo(1), "CurrectContactProvider.GetCurrentContact should be called."),
                () => Assert.That(() => mCurrectContactProvider.SetCurrentContactFlag, Is.EqualTo(1), "CurrectContactProvider.SetCurrentContact should be called.")
            );
        }


        [Test]
        public async Task GetCurrentContactAsync_NoExistingContact_ReturnsNull()
        {
            // Act
            var contact = await mContactTrackingService.GetCurrentContactAsync("NotExisting");

            // Assert
            Assert.That(contact, Is.Null);
        }


        [Test]
        public async Task GetCurrentContactAsync_ContactExists_ReturnsExistingContact()
        {
            // Arrange
            var expectedContact = new ContactInfo
            {
                ContactLastName = "TestContact"
            };
            mCurrectContactProvider.CurrentContact = expectedContact;

            // Act
            var contact = await mContactTrackingService.GetCurrentContactAsync("NotExisting");

            // Assert
            Assert.That(contact, Is.EqualTo(expectedContact));
        }
    }
}
