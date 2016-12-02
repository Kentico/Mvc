using System;
using System.Web;

using CMS.Helpers;
using CMS.Tests;

using NSubstitute;

using NUnit.Framework;

namespace Kentico.Activities.Tests
{
    [TestFixture]
    public class ActivityMvcUrlServiceTests : UnitTests
    {
        private ActivityMvcUrlService mService;
        private const string URL = "http://localhost";
        private const string URL_REFERRER = "http://www.example.com/";
        private const string USER_HOST_ADDRESS = "8.8.8.8";


        [SetUp]
        public void SetUp()
        {
            CMSHttpContext.Current = Substitute.For<HttpContextBase>();
            CMSHttpContext.Current.Request.Url.Returns(new Uri(URL));
            CMSHttpContext.Current.Request.RawUrl.Returns(URL);
            CMSHttpContext.Current.Request.UrlReferrer.Returns(new Uri(URL_REFERRER));
            RequestContext.UserHostAddress = USER_HOST_ADDRESS;

            mService = new ActivityMvcUrlService();
        }


        [Test]
        public void GetActivityUrl_WithRequest_ReturnsCorrectUrl()
        {
            // Act
            var url = mService.GetActivityUrl();

            // Assert
            Assert.That(url, Is.EqualTo(URL_REFERRER), "URL is not correctly set to referrer.");
        }


        [Test]
        public void GetActivityUrlReferrer_WithRequest_ReturnsCorrectUrlReferrer()
        {
            // Act
            var referrer = mService.GetActivityUrlReferrer();

            // Assert
            Assert.That(referrer, Is.Empty, "URL referrer is not empty.");
        }
    }
}
