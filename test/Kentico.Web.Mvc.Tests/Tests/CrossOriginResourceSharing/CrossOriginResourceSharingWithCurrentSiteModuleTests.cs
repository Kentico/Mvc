using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Tests;

using NSubstitute;

using NUnit.Framework;

namespace Kentico.Web.Mvc.Tests
{
    public class CrossOriginResourceSharingWithCurrentSiteModuleTests
    {
        [TestFixture]
        public class SetHeaderToAllowCurrentSiteOrigin : UnitTests
        {
            private const string PROTOCOL = "http://";
            private const string ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME = "Access-Control-Allow-Origin";
            private const string ORIGIN_HEADER_NAME = "Origin";


            private const string CURRENT_SITE_NAME_PREFIX = "Current";
            private const string ANOTHER_SITE_NAME_PREFIX = "Another";

            private const string SITE_NAME = "SiteName";
            private const string SITE_DOMAIN = "SiteDomainName.com";
            private const string SITE_DOMAIN_ALIAS_ONE = "SiteDomainAliasOne.com";
            private const string SITE_DOMAIN_ALIAS_TWO = "SiteDomainAliasTwo.com";
            private const string SITE_PRESENTATION_URL = "SitePresentationUrl.com/MvcSite";

            private const string UNKNOWN_SITE_DOMAIN = "UnknownSiteDomain.com";
            private const string SOME_DIFFERENT_DOMAIN = "SomeDifferentDomain.com";


            private FakeSiteInfoProvider mFakeSiteInfoProvider;
            private List<SiteDomainAliasInfo> mSiteDomainAliases = new List<SiteDomainAliasInfo>();


            private static HttpContextBase mHttpContext;


            [SetUp]
            public void SetUp()
            {
                Fake<SiteInfo>();
                Fake<SiteDomainAliasInfo>();

                var siteId = 0;
                var currentSiteInfo = CreateSiteInfo(++siteId, CURRENT_SITE_NAME_PREFIX);
                var anotherSiteInfo = CreateSiteInfo(++siteId, ANOTHER_SITE_NAME_PREFIX);

                mFakeSiteInfoProvider = new FakeSiteInfoProvider();
                Fake<SiteInfo, SiteInfoProvider>(mFakeSiteInfoProvider).WithData(currentSiteInfo, anotherSiteInfo);
                Fake<SiteDomainAliasInfo, SiteDomainAliasInfoProvider>().WithData(mSiteDomainAliases.ToArray());

                SiteContext.CurrentSite = currentSiteInfo;

                mHttpContext = CreateHttpContext();
            }


            private SiteInfo CreateSiteInfo(int siteId, string prefix)
            {
                var siteDomainAliasOne = prefix + SITE_DOMAIN_ALIAS_ONE;
                var siteDomainAliasTwo = prefix + SITE_DOMAIN_ALIAS_TWO;
                mSiteDomainAliases.Add(new SiteDomainAliasInfo { SiteID = siteId, SiteDomainAliasName = siteDomainAliasOne });
                mSiteDomainAliases.Add(new SiteDomainAliasInfo { SiteID = siteId, SiteDomainAliasName = siteDomainAliasTwo });

                return new SiteInfo
                {
                    SiteID = siteId,
                    SiteName = prefix + SITE_NAME,
                    SitePresentationURL = prefix + SITE_PRESENTATION_URL,
                    DomainName = prefix + SITE_DOMAIN                    
                };
            }


            private static HttpContextBase CreateHttpContext()
            {
                var requestHeaders = new NameValueCollection();
                var request = Substitute.For<HttpRequestBase>();
                request.Headers.Returns(requestHeaders);

                var responseHeaders = new NameValueCollection();
                var response = Substitute.For<HttpResponseBase>();
                response.Headers.Returns(responseHeaders);

                // Fake AppendHeader method is necessary for adding the header into collection from the tested code
                response
                    .WhenForAnyArgs(r => r.AppendHeader(Arg.Any<string>(), Arg.Any<string>()))
                    .Do(c => response.Headers[c.ArgAt<string>(0)] = c.ArgAt<string>(1));

                var httpContext = Substitute.For<HttpContextBase>();
                httpContext.Request.Returns(request);
                httpContext.Response.Returns(response);

                return httpContext;
            }


            [Test]
            public void OnActionExecuting_CrossOriginRequestFromUnknownSite_AccessControlAllowOriginHeaderIsNotInResponse()
            {
                var originUri = PROTOCOL + UNKNOWN_SITE_DOMAIN;
                mHttpContext.Request.Headers.Add(ORIGIN_HEADER_NAME, originUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.False(mHttpContext.Response.Headers.AllKeys.Contains(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME), 
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} shouldn't be present in response " +
                    $"when URI from request header {ORIGIN_HEADER_NAME} doesn't belong to any CMS site.");
            }


            [Test]
            public void OnActionExecuting_CrossOriginRequestFromUnknownSite_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse()
            {
                var originUri = PROTOCOL + UNKNOWN_SITE_DOMAIN;
                var someDifferentUri = PROTOCOL + SOME_DIFFERENT_DOMAIN;
                mHttpContext.Request.Headers.Add(ORIGIN_HEADER_NAME, originUri);
                mHttpContext.Response.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME, someDifferentUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.AreEqual(someDifferentUri, mHttpContext.Response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME],
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} must be kept in response (if it was added previously by something else) " +
                    $"when URI from request header {ORIGIN_HEADER_NAME} doesn't belong any CMS site.");
            }


            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_DOMAIN, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSiteDomain_AccessControlAllowOriginHeaderIsNotInResponse")]
            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_ONE, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSiteDomainAliasOne_AccessControlAllowOriginHeaderIsNotInResponse")]
            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_TWO, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSiteDomainAliasTwo_AccessControlAllowOriginHeaderIsNotInResponse")]
            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_PRESENTATION_URL, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSitePresentationUrl_AccessControlAllowOriginHeaderIsNotInResponse")]
            public void OnActionExecuting_CrossOriginRequestFromAnotherSite_AccessControlAllowOriginHeaderIsNotInResponse(string originUri)
            {
                mHttpContext.Request.Headers.Add(ORIGIN_HEADER_NAME, originUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.False(mHttpContext.Response.Headers.AllKeys.Contains(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME),
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} shouldn't be present in response " +
                    $"when URI from request header {ORIGIN_HEADER_NAME} doesn't belong current CMS site.");
            }


            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_DOMAIN, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSiteDomain_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse")]
            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_ONE, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSiteDomainAliasOne_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse")]
            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_TWO, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSiteDomainAliasTwo_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse")]
            [TestCase(PROTOCOL + ANOTHER_SITE_NAME_PREFIX + SITE_PRESENTATION_URL, TestName = "OnActionExecuting_CrossOriginRequestFromAnotherSitePresentationUrl_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse")]
            public void OnActionExecuting_CrossOriginRequestFromAnotherSite_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse(string originUri)
            {
                var someDifferentUri = PROTOCOL + SOME_DIFFERENT_DOMAIN;
                mHttpContext.Request.Headers.Add(ORIGIN_HEADER_NAME, originUri);
                mHttpContext.Response.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME, someDifferentUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.AreEqual(someDifferentUri, mHttpContext.Response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME],
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} must be kept in response (if it was added previously by something else) " +
                    $"when URI from request header {ORIGIN_HEADER_NAME} doesn't belong current CMS site.");
            }


            [Test]
            public void OnActionExecuting_StandardRequestWithoutOrigin_AccessControlAllowOriginHeaderIsNotInResponse()
            {
                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.False(mHttpContext.Response.Headers.AllKeys.Contains(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME),
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} shouldn't be present in response " +
                    $"when header {ORIGIN_HEADER_NAME} is not present in the request.");
            }


            [Test]
            public void OnActionExecuting_StandardRequestWithoutOrigin_ShouldNotTouchExistingAccessControlAllowOriginHeaderInResponse()
            {
                var someDifferentUri = PROTOCOL + SOME_DIFFERENT_DOMAIN;
                mHttpContext.Response.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME, someDifferentUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.AreEqual(someDifferentUri, mHttpContext.Response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME],
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} must be kept in response (if it was added previously by something else) " +
                    $"when header {ORIGIN_HEADER_NAME} is not present in the request");
            }


            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_DOMAIN, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSiteDomain_AccessControlAllowOriginHeaderIsInResponseWithOriginValue")]
            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_ONE, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSiteDomainAliasOne_AccessControlAllowOriginHeaderIsInResponseWithOriginValue")]
            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_TWO, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSiteDomainAliasTwo_AccessControlAllowOriginHeaderIsInResponseWithOriginValue")]
            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_PRESENTATION_URL, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSitePresentationUrl_AccessControlAllowOriginHeaderIsInResponseWithOriginValue")]
            public void OnActionExecuting_CrossOriginRequestFromCurrentSite_AccessControlAllowOriginHeaderIsInResponseWithOriginValue(string originUri)
            {
                mHttpContext.Request.Headers.Add(ORIGIN_HEADER_NAME, originUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.AreEqual(originUri, mHttpContext.Response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME],
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} must be present in response " +
                    $"when URI from request header {ORIGIN_HEADER_NAME} belongs to current CMS site.");
            }


            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_DOMAIN, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSiteDomainAndAccessControlHeaderAlreadySet_AccessControlAllowOriginHeaderIsInResponseOverwrittenWithOriginValue")]
            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_ONE, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSiteDomainAliasOneAndAccessControlHeaderAlreadySet_AccessControlAllowOriginHeaderIsInResponseOverwrittenWithOriginValue")]
            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_DOMAIN_ALIAS_TWO, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSiteDomainAliasTwoAndAccessControlHeaderAlreadySet_AccessControlAllowOriginHeaderIsInResponseOverwrittenWithOriginValue")]
            [TestCase(PROTOCOL + CURRENT_SITE_NAME_PREFIX + SITE_PRESENTATION_URL, TestName = "OnActionExecuting_CrossOriginRequestFromCurrentSitePresentationUrlAndAccessControlHeaderAlreadySet_AccessControlAllowOriginHeaderIsInResponseOverwrittenWithOriginValue")]
            public void OnActionExecuting_CrossOriginRequestFromCurrentSiteAndAccessControlHeaderAlreadySet_AccessControlAllowOriginHeaderIsInResponseOverwrittenWithOriginValue(string originUri)
            {
                var someDifferentUri = PROTOCOL + SOME_DIFFERENT_DOMAIN;
                mHttpContext.Request.Headers.Add(ORIGIN_HEADER_NAME, originUri);
                mHttpContext.Response.Headers.Add(ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME, someDifferentUri);

                CrossOriginResourceSharingWithCurrentSiteModule.SetHeaderToAllowCurrentSiteOrigin(mHttpContext);

                Assert.AreEqual(originUri, mHttpContext.Response.Headers[ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME],
                    $"Header {ACCESS_CONTROL_ALLOW_ORIGIN_HEADER_NAME} must be overwritten in response " +
                    $"when URI from request header {ORIGIN_HEADER_NAME} belongs to current CMS site.");
            }
        }


        private class FakeSiteInfoProvider : SiteInfoProvider
        {
            public FakeSiteInfoProvider()
            {
            }


            protected override string GetSiteNameFromUrlInternal(string url)
            {
                string domain = URLHelper.GetDomain(url);
                var siteBoundWithUrl = GetSites()
                    .FirstOrDefault(s => 
                        String.Equals(s.DomainName, domain, StringComparison.InvariantCultureIgnoreCase) ||
                        String.Equals(s.SitePresentationDomain, domain, StringComparison.InvariantCultureIgnoreCase) ||
                        s.SiteDomainAliases[domain] != null);

                return siteBoundWithUrl?.SiteName;
            }
        }
    }
}