using System;
using System.IO;
using System.Reflection;

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using CMS.Tests;

using NUnit.Framework;
using NSubstitute;

namespace Kentico.Web.Mvc.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class HtmlHelperResolveUrlsMethodsTests
    {
        private const string APPLICATIONPATH = "/MVCDemo/SomeVirtualFolder";
        private const string DATA_PATH = @"Data\HelperMethods\";
        private EmbeddedResourceReader mEmbeddedResourceReader;
        private HtmlHelper mHtmlHelper;
        private string mUnresolvedHtml;
        private string mResolvedHtml;


        /// <summary>
        /// Provides ability to read embedded resource files from (executing) assembly.
        /// </summary>
        protected EmbeddedResourceReader EmbeddedResourceReader
        {
            get
            {
                return mEmbeddedResourceReader ?? (mEmbeddedResourceReader = new EmbeddedResourceReader(Assembly.GetAssembly(GetType())));
            }
        }


        [SetUp]
        public void SetUp()
        {
            mUnresolvedHtml = EmbeddedResourceReader.ReadResourceFile(DATA_PATH, "HtmlHelperResolveUrlsMethodsTests_UnResolved.html");
            mResolvedHtml = EmbeddedResourceReader.ReadResourceFile(DATA_PATH, "HtmlHelperResolveUrlsMethodsTests_Resolved.html");
            mHtmlHelper = CreateHtmlHelper();
        }


        [Test]
        public void ResolveUrls_UnResolvedHtml_ResolvesUrlsCorrectly()
        {
            var result = mHtmlHelper.Kentico().ResolveUrls(mUnresolvedHtml).ToString();
            Assert.AreEqual(mResolvedHtml, result);
        }


        [Test]
        [TestCase("(~/Folder/File.jpg)", "(/MVCDemo/SomeVirtualFolder/Folder/File.jpg)")]
        [TestCase("'~/Folder/File.jpg'", "'/MVCDemo/SomeVirtualFolder/Folder/File.jpg'")]
        [TestCase(@"""~/Folder/File.jpg""", @"""/MVCDemo/SomeVirtualFolder/Folder/File.jpg""")]
        public void ResolveUrls_ResolveSingleUrl_Correctly(string input, string expectedOutput)
        {
            input = mHtmlHelper.Kentico().ResolveUrls(input).ToString();
            Assert.AreEqual(expectedOutput, input);
        }


        [Test]
        public void ResolveUrls_NullExtensionPoint_ShouldThrow()
        {
            Assert.That(() => ((ExtensionPoint<HtmlHelper>)null).ResolveUrls(String.Empty),
                Throws.Exception
                    .TypeOf<ArgumentNullException>());
        }


        private static HtmlHelper CreateHtmlHelper()
        {
            var context = Substitute.For<HttpContextBase>();
            context.Request.ApplicationPath.Returns(APPLICATIONPATH);

            var viewDataDictionary = new ViewDataDictionary();
            var controllerContext = new ControllerContext(context,
                                                          new RouteData(),
                                                          Substitute.For<ControllerBase>());

            var viewContext = new ViewContext(controllerContext, Substitute.For<IView>(), viewDataDictionary, new TempDataDictionary(), Substitute.For<TextWriter>());

            var mockViewDataContainer = Substitute.For<IViewDataContainer>();
            mockViewDataContainer.ViewData.Returns(viewDataDictionary);

            return new HtmlHelper(viewContext, mockViewDataContainer);
        }
    }
}
