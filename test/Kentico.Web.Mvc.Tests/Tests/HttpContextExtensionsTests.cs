using System.IO;
using System.Web;
using System.Collections.Generic;

using NSubstitute;
using NUnit.Framework;

namespace Kentico.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class HttpContextExtensions.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class HttpContextExtensionsTests
    {
        private const string DOMAIN_URL = "http://www.domain.tld";
        private const string FEATURE_BUNDLE_KEY = "Kentico.Features";


        [Test]
        public void Kentico_HttpContext_ResultNotNull()
        {
            HttpContext context = GetHttpContext(DOMAIN_URL);
            IFeatureSet resultFeatures = context.Kentico();

            Assert.That(resultFeatures, Is.Not.Null);
        }


        [Test]
        public void Kentico_HttpContextBase_ResultNotNull()
        {
            HttpContextBase context = Substitute.For<HttpContextBase>();
            context.Items.Returns(new Dictionary<string, IFeatureSet>());

            IFeatureSet features = context.Kentico();
            Assert.That(features, Is.Not.Null);
        }


        [Test]
        public void Kentico_ContextItemsNotEmpty_CorrectResult()
        {
            var context = Substitute.For<HttpContextBase>();
            var featuresMock = Substitute.For<IFeatureSet>();
            context.Items.Returns(new Dictionary<string, IFeatureSet>()
            {
                {FEATURE_BUNDLE_KEY, featuresMock}
            });

            IFeatureSet resultFeatures = context.Kentico();
            Assert.That(resultFeatures, Is.EqualTo(featuresMock));
        }


        private HttpContext GetHttpContext(string domain)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            return new HttpContext(new HttpRequest("dummy.aspx", domain, string.Empty), new HttpResponse(writer));
        }
    }
}
