using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using NSubstitute;

namespace Kentico.Web.Mvc.Tests
{
    [TestFixture]
    public class UrlHelperResolveUrlsTests
    {
        private string mUnresolvedHtml;
        private string mResolvedHtml;
        private const string APPLICATIONPATH = "/MVCDemo/SomeVirtualFolder";
        private HtmlHelper mHtmlHelper;


        [SetUp]
        public void SetUp()
        {
            mUnresolvedHtml = File.ReadAllText(@".\UrlHelperResolveUrlsTests\UnResolved.html");
            mResolvedHtml = File.ReadAllText(@".\UrlHelperResolveUrlsTests\Resolved.html");
            mHtmlHelper = CreateHtmlHelper();
        }


        [Test]
        public void UrlHelper_ResolveUrls_Correctly()
        {
            var result = mHtmlHelper.Kentico().ResolveUrls(mUnresolvedHtml).ToString();
            Assert.AreEqual(mResolvedHtml, result);
        }


        [Test]
        [TestCase("(~/Folder/File.jpg)", "(/MVCDemo/SomeVirtualFolder/Folder/File.jpg)")]
        [TestCase("'~/Folder/File.jpg'", "'/MVCDemo/SomeVirtualFolder/Folder/File.jpg'")]
        [TestCase(@"""~/Folder/File.jpg""", @"""/MVCDemo/SomeVirtualFolder/Folder/File.jpg""")]
        public void UrlHelper_ResolveSingleUrl_Correctly(string input, string expectedOutput)
        {
            input = mHtmlHelper.Kentico().ResolveUrls(input).ToString();
            Assert.AreEqual(expectedOutput, input);
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
