using System;
using System.Web.Routing;

using CMS.Tests;
using CMS.Newsletters;

using Kentico.Web.Mvc;

using NUnit.Framework;

namespace Kentico.Newsletters.Web.Mvc.Tests
{
    [TestFixture]
    public class RouteCollectionExtensionsTests
    {
        [Category.Unit]
        [TestFixture]
        public class MapEmailOpenedTrackingRouteTests
        {
            private ExtensionPoint<RouteCollection> routeCollection;


            [SetUp]
            public void SetUp()
            {
                routeCollection = new RouteCollection().Kentico();
            }


            [Test]
            public void MapEmailOpenedTrackingRoute_DefaultRoute_RouteIsRegisteredWithDefaultRoute()
            {
                var route = routeCollection.MapOpenedEmailHandlerRoute();

                CMSAssert.All(
                    () => Assert.IsNotNull(route),
                    () => CollectionAssert.Contains(routeCollection.Target, route),
                    () => Assert.AreEqual(1, routeCollection.Target.Count),
                    //() => Assert.AreEqual(EmailTrackingLinkHelper.DEFAULT_OPENED_EMAIL_TRACKING_ROUTE_HANDLER_URL, route.Url),
                    () => Assert.IsInstanceOf<RouteHandlerWrapper<OpenEmailTracker>>(route.RouteHandler),
                    () => Assert.AreEqual(String.Empty, route.Constraints["controller"]),
                    () => Assert.AreEqual(String.Empty, route.Constraints["action"]));
            }


            [Test]
            public void MapEmailOpenedTrackingRoute_CustomRoute_RouteIsRegisteredWithDefaultRoute()
            {
                string myCustomRoute = "custom/route";
                var route = routeCollection.MapOpenedEmailHandlerRoute(myCustomRoute);

                CMSAssert.All(
                    () => Assert.IsNotNull(route),
                    () => CollectionAssert.Contains(routeCollection.Target, route),
                    () => Assert.AreEqual(1, routeCollection.Target.Count),
                    () => Assert.AreEqual(myCustomRoute, route.Url),
                    () => Assert.IsInstanceOf<RouteHandlerWrapper<OpenEmailTracker>>(route.RouteHandler),
                    () => Assert.AreEqual(String.Empty, route.Constraints["controller"]),
                    () => Assert.AreEqual(String.Empty, route.Constraints["action"]));
            }
        }


        [Category.Unit]
        [TestFixture]
        public class MapClickedLinkInEmailHandlerToRouteTests
        {
            private ExtensionPoint<RouteCollection> routeCollection;


            [SetUp]
            public void SetUp()
            {
                routeCollection = new RouteCollection().Kentico();
            }


            [Test]
            public void MapClickedLinkInEmailHandlerToRoute_DefaultRoute_RouteIsRegisteredWithDefaultRoute()
            {
                var route = routeCollection.MapEmailLinkHandlerRoute();

                CMSAssert.All(
                    () => Assert.IsNotNull(route),
                    () => CollectionAssert.Contains(routeCollection.Target, route),
                    () => Assert.AreEqual(1, routeCollection.Target.Count),
                    //() => Assert.AreEqual(EmailTrackingLinkHelper.DEFAULT_LINKS_TRACKING_ROUTE_HANDLER_URL, route.Url),
                    () => Assert.IsInstanceOf<RouteHandlerWrapper<LinkTracker>>(route.RouteHandler),
                    () => Assert.AreEqual(String.Empty, route.Constraints["controller"]),
                    () => Assert.AreEqual(String.Empty, route.Constraints["action"]));
            }


            [Test]
            public void MapClickedLinkInEmailHandlerToRoute_CustomRoute_RouteIsRegisteredWithDefaultRoute()
            {
                string myCustomRoute = "custom/route";
                var route = routeCollection.MapEmailLinkHandlerRoute(myCustomRoute);

                CMSAssert.All(
                    () => Assert.IsNotNull(route),
                    () => CollectionAssert.Contains(routeCollection.Target, route),
                    () => Assert.AreEqual(1, routeCollection.Target.Count),
                    () => Assert.AreEqual(myCustomRoute, route.Url),
                    () => Assert.IsInstanceOf<RouteHandlerWrapper<LinkTracker>>(route.RouteHandler),
                    () => Assert.AreEqual(String.Empty, route.Constraints["controller"]),
                    () => Assert.AreEqual(String.Empty, route.Constraints["action"]));
            }
        }
    }
}
