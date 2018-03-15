using System.Linq;
using System.Web.Mvc;

using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Content.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class <see cref="PreviewGlobalFiltersTests"/>.
    /// </summary>
    public class PreviewGlobalFiltersTests
    {
        [TestFixture]
        [Category.Unit]
        public class RegisterTests
        {
            [SetUp]
            public void SetUp()
            {
                GlobalFilters.Filters.Clear();
            }


            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                GlobalFilters.Filters.Clear();
            }


            [Test]
            public void Register_SingleCall_PreviewFilterRegistered()
            {
                PreviewGlobalFilters.Register();

                CMSAssert.All(
                    () => Assert.That(GlobalFilters.Filters.Count, Is.EqualTo(1)),
                    () => Assert.That(GlobalFilters.Filters.First().Instance, Is.TypeOf(typeof(PreviewOutputCacheFilter)))
                );
            }


            [Test]
            public void Register_TwoCalls_PreviewFilterRegisteredOnlyOnce()
            {
                PreviewGlobalFilters.Register();
                PreviewGlobalFilters.Register();

                CMSAssert.All(
                    () => Assert.That(GlobalFilters.Filters.Count, Is.EqualTo(1)),
                    () => Assert.That(GlobalFilters.Filters.First().Instance, Is.TypeOf(typeof(PreviewOutputCacheFilter)))
                );
            }

        }
    }
}
