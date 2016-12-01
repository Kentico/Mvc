using System;
using System.Linq;
using System.Web.Routing;

using CMS.Routing.Web;
using CMS.Tests;

using NSubstitute;
using NUnit.Framework;

namespace Kentico.Web.Mvc.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class RouteCollectionAddRoutesMethodsTests
    {
        /// <summary>
        /// Dummy class for test purposes.
        /// </summary>
        private class TestHandler
        {
        };


        [Test]
        public void MapRoutes_ValidRoute_RouteGetsMapped()
        {
            var routes = Substitute.For<RouteCollection>();
            var handler = new RegisterHttpHandlerAttribute("TestRoute/SomeVirtualPath", typeof (TestHandler));
            handler.PreInit();

            routes.Kentico().MapRoutes();
            var registeredRoutes = HttpHandlerRouteTable.Default.GetRoutes().ToList();
            var actualRoutes = registeredRoutes.Where(i => i.Url == handler.RouteTemplate).ToList();

            CMSAssert.All(
                () => Assert.That(actualRoutes.Count, Is.EqualTo(1)),
                () => Assert.That(actualRoutes.First().Url, Is.EqualTo(handler.RouteTemplate)));
        }


        [Test]
        public void MapRoutes_NullExtensionPoint_ThrowsException()
        {
            Assert.That(() => ((ExtensionPoint<RouteCollection>)null).MapRoutes(),
                Throws.Exception
                    .TypeOf<ArgumentNullException>());
        }
    }
}
