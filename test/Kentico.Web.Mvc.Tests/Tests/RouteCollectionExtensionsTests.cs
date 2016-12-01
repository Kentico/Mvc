using System.Web.Routing;

using NUnit.Framework;

namespace Kentico.Web.Mvc.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class RouteCollectionExtensionsTests
    {
        [Test]
        public void Kentico_ValidArgument_ResultIsNotNull()
        {
            var routes = new RouteCollection();
            ExtensionPoint<RouteCollection> extensionPoint = routes.Kentico();
            Assert.IsNotNull(extensionPoint);
        }
    }
}
