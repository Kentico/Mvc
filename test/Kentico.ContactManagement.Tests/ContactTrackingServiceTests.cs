using System.Threading.Tasks;

using CMS.Base;
using CMS.ContactManagement;
using CMS.ContactManagement.Internal;
using CMS.Core;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Tests;
using NSubstitute;
using NUnit.Framework;

namespace Kentico.ContactManagement.Tests
{
    [TestFixture]
    [Category.Unit]
    public class ContactTrackingServiceTests : UnitTests
    {
        private ICurrentContactProvider mCurrectContactProvider;
        private IContactTrackingService mContactTrackingService;
        private IContactProcessingChecker mContactProcessingChecker;

        [SetUp]
        public void SetUp()
        {
            mCurrectContactProvider = Substitute.For<ICurrentContactProvider>();
            Service.Use<ICurrentContactProvider>(mCurrectContactProvider);

            var site = Substitute.For<SiteInfo>();
            site.SiteName.Returns("TestSite");

            var siteService = Substitute.For<ISiteService>();
            siteService.CurrentSite.Returns(site);
            siteService.IsLiveSite.Returns(false);
            Service.Use<ISiteService>(siteService);

            mContactProcessingChecker = Substitute.For<IContactProcessingChecker>();
            mContactProcessingChecker.CanProcessContactInCurrentContext().Returns(true);
            Service.Use<IContactProcessingChecker>(mContactProcessingChecker);

            mContactTrackingService = new ContactTrackingService();
            
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
        public async Task GetCurrentContactAsync_CannotProcessContact_ReturnsNull()
        {
            mContactProcessingChecker.CanProcessContactInCurrentContext().Returns(false);
            // Act
            var result = await mContactTrackingService.GetCurrentContactAsync(string.Empty);

            // Assert
            CMSAssert.All(
                () => Assert.That(result, Is.Null, "Result contact should be null"),
                () => mCurrectContactProvider.DidNotReceive().GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()),
                () => mCurrectContactProvider.DidNotReceive().SetCurrentContact(Arg.Any<ContactInfo>())
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
            
            mCurrectContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), false).Returns(expectedContact);

            // Act
            var result = await mContactTrackingService.GetCurrentContactAsync(userName);

            // Assert
            CMSAssert.All(
                () => Assert.That(result, Is.SameAs(expectedContact), "Result contact should be the same as the expected one"),
                () => mCurrectContactProvider.Received(1).GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()),
                () => mCurrectContactProvider.Received(1).SetCurrentContact(Arg.Any<ContactInfo>())
            );
        }


        [Test]
        public async Task MergeUserContactsAsync_ConnotProcessContact_ReturnsNull()
        {
            // Arrange
            mContactProcessingChecker.CanProcessContactInCurrentContext().Returns(false);

            // Act
            await mContactTrackingService.MergeUserContactsAsync(string.Empty);

            // Assert
            CMSAssert.All(
                () => mCurrectContactProvider.DidNotReceive().GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()),
                () => mCurrectContactProvider.DidNotReceive().SetCurrentContact(Arg.Any<ContactInfo>())
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

            mCurrectContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), false).Returns(expectedContact);

            // Act
            await mContactTrackingService.MergeUserContactsAsync(userName);

            // Assert
            CMSAssert.All(
                () => mCurrectContactProvider.Received(1).GetCurrentContact(Arg.Any<IUserInfo>(), Arg.Any<bool>()),
                () => mCurrectContactProvider.Received(1).SetCurrentContact(Arg.Any<ContactInfo>())
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
            mCurrectContactProvider.GetCurrentContact(Arg.Any<IUserInfo>(), false).Returns(expectedContact);

            // Act
            var contact = await mContactTrackingService.GetCurrentContactAsync("NotExisting");

            // Assert
            Assert.That(contact, Is.EqualTo(expectedContact));
        }
    }
}
