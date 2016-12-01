using System;

using NSubstitute;
using NUnit.Framework;

namespace Kentico.Web.Mvc.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class ApplicationBuilderTests
    {
        [Test]
        public void RegisterModule_NullArgument_ThrowsException()
        {
            Assert.That(() => ApplicationBuilder.Current.RegisterModule(null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>());
        }


        [Test]
        public void RegisterModule_ValidArgument_DoesNotThrowException()
        {
            var module = Substitute.For<IModule>();
            Assert.DoesNotThrow(() => ApplicationBuilder.Current.RegisterModule(module));
        }
    }
}
