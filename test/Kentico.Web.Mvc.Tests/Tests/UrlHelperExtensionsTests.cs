using System.Web.Mvc;

using NSubstitute;
using NUnit.Framework;

namespace Kentico.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class UrlHelperExtensions.
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    public class UrlHelperExtensionsTests
    {
        [Test]
        public void Kentico_ValidArgument_ResultIsNotNull()
        {
            var urlHelperMock = Substitute.For<UrlHelper>();

            Assert.That(urlHelperMock.Kentico(), Is.Not.Null);
        }
    }
}
