using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using CMS.Tests;
using Kentico.Web.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Kentico.Content.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class UrlHelperFileMethods.
    /// </summary>
    public class UrlHelperFileMethodsTests
    {
        private const string APPLICATION_PATH = "/MVCDemo/SomeVirtualFolder/";


        private static HttpContextBase MockHttpContext()
        {
            var context = Substitute.For<HttpContextBase>();
            context.Request.ApplicationPath.Returns(APPLICATION_PATH);
            return context;
        }


        [TestFixture]
        public class FileUrlTests : UnitTests
        {
            private const string FILE_PATH = APPLICATION_PATH + "VirtualFile.pdf";
            private const string FILE_PATH_WITH_QUERY = APPLICATION_PATH + "GetAzureFile.aspx?filename=file.jpg";
            private UrlHelper mUrlHelper;


            [SetUp]
            public void SetUp()
            {
                var context = MockHttpContext();
                mUrlHelper = new UrlHelper(new RequestContext(context, new RouteData()));
            }


            [Test]
            public void FileUrl_NullExtensionPoint_ThrowsException()
            {
                Assert.That(
                    () => ((ExtensionPoint<UrlHelper>)null).FileUrl(FILE_PATH, null), Throws.Exception.TypeOf<ArgumentNullException>()
                );
            }


            [Test]
            public void FileUrl_NullPath_ThrowsException()
            {
                Assert.That(
                    () => mUrlHelper.Kentico().FileUrl(null, null), Throws.Exception.TypeOf<ArgumentException>()
                );
            }


            [Test]
            public void FileUrl_EmptyPath_ThrowsException()
            {
                Assert.That(
                    () => mUrlHelper.Kentico().FileUrl(string.Empty, null), Throws.Exception.TypeOf<ArgumentException>()
                );
            }


            [TestCase(null, "/MVCDemo/SomeVirtualFolder/VirtualFile.pdf")]
            [TestCase(false, "/MVCDemo/SomeVirtualFolder/VirtualFile.pdf")]
            [TestCase(true, "/MVCDemo/SomeVirtualFolder/VirtualFile.pdf?disposition=attachment")]
            public void FileUrl_WithUrlOptions_ResolvesUrlCorrectly(bool contentDisposition, string expectedUrl)
            {
                var urlOptions = new FileUrlOptions { AttachmentContentDisposition = contentDisposition };
                string url = mUrlHelper.Kentico().FileUrl(FILE_PATH, urlOptions);

                Assert.That(url, Is.EqualTo(expectedUrl));
            }

            [TestCase(FILE_PATH_WITH_QUERY, null, "/MVCDemo/SomeVirtualFolder/GetAzureFile.aspx?filename=file.jpg")]
            [TestCase(FILE_PATH_WITH_QUERY, false, "/MVCDemo/SomeVirtualFolder/GetAzureFile.aspx?filename=file.jpg")]
            [TestCase(FILE_PATH_WITH_QUERY, true, "/MVCDemo/SomeVirtualFolder/GetAzureFile.aspx?filename=file.jpg&disposition=attachment")]
            public void FileUrl_PathWithQueryString_ResolvesUrlWithCombinedQueryStringCorrectly(string path, bool contentDisposition, string expectedUrl)
            {
                var urlOptions = new FileUrlOptions { AttachmentContentDisposition = contentDisposition };
                string url = mUrlHelper.Kentico().FileUrl(path, urlOptions);

                Assert.That(url, Is.EqualTo(expectedUrl));
            }
        }


        [TestFixture]
        public class ImageUrlTests : UnitTests
        {
            private const string IMAGE_PATH = APPLICATION_PATH + "VirtualFile.png";
            private const string IMAGE_PATH_WITH_QUERY = APPLICATION_PATH + "GetAzureFile.aspx?filename=file.jpg";
            private UrlHelper mUrlHelper;


            [SetUp]
            public void SetUp()
            {
                var context = MockHttpContext();
                mUrlHelper = new UrlHelper(new RequestContext(context, new RouteData()));
            }


            [Test]
            public void ImageUrl_NullExtensionPoint_ThrowsException()
            {
                Assert.That(
                    () => ((ExtensionPoint<UrlHelper>)null).FileUrl(IMAGE_PATH, null), Throws.Exception.TypeOf<ArgumentNullException>()
                );
            }


            [Test]
            public void ImageUrl_NullPath_ThrowsException()
            {
                Assert.That(
                    () => mUrlHelper.Kentico().FileUrl(null, null), Throws.Exception.TypeOf<ArgumentException>()
                );
            }


            [Test]
            public void ImageUrl_EmptyPath_ThrowsException()
            {
                Assert.That(
                    () => mUrlHelper.Kentico().FileUrl(string.Empty, null), Throws.Exception.TypeOf<ArgumentException>()
                );
            }


            public static IEnumerable<TestCaseData> SizeConstraintsParameters
            {
                get
                {
                    yield return new TestCaseData(SizeConstraint.Empty, "/MVCDemo/SomeVirtualFolder/VirtualFile.png").SetName("Empty size constraint.");
                    yield return new TestCaseData(SizeConstraint.Size(100, 200), "/MVCDemo/SomeVirtualFolder/VirtualFile.png?width=100&height=200&resizemode=force").SetName("Size constraint defined both by width and height.");
                    yield return new TestCaseData(SizeConstraint.Width(300), "/MVCDemo/SomeVirtualFolder/VirtualFile.png?width=300&resizemode=force").SetName("Size constraint defined by width.");
                    yield return new TestCaseData(SizeConstraint.Height(500), "/MVCDemo/SomeVirtualFolder/VirtualFile.png?height=500&resizemode=force").SetName("Size constraint defined by height.");
                    yield return new TestCaseData(SizeConstraint.MaxWidthOrHeight(600), "/MVCDemo/SomeVirtualFolder/VirtualFile.png?maxsidesize=600&resizemode=force").SetName("Size constraint defined by max width or height.");
                }
            }


            public static IEnumerable<TestCaseData> ImagePathWithQuerySizeConstraintsParameters
            {
                get
                {
                    yield return new TestCaseData(SizeConstraint.Empty, "/MVCDemo/SomeVirtualFolder/GetAzureFile.aspx?filename=file.jpg").SetName("Image path with query string & Empty size constraint .");
                    yield return new TestCaseData(SizeConstraint.Size(100, 200), "/MVCDemo/SomeVirtualFolder/GetAzureFile.aspx?filename=file.jpg&width=100&height=200&resizemode=force").SetName("Image path with query string & Size constraint defined both by width and height.");
                }
            }


            [Test, TestCaseSource(nameof(ImagePathWithQuerySizeConstraintsParameters))]
            public void ImageUrl_SizeConstraints_ResolvesUrlCorrectly(SizeConstraint sizeConstraint, string expectedUrl)
            {
                string attachmentUrl = mUrlHelper.Kentico().ImageUrl(IMAGE_PATH_WITH_QUERY, sizeConstraint);

                Assert.That(attachmentUrl, Is.EqualTo(expectedUrl));
            }


            [Test, TestCaseSource(nameof(SizeConstraintsParameters))]
            public void ImageUrl_SizeConstraints_ResolvesUrlWithCombinedQueryStringCorrectly(SizeConstraint sizeConstraint, string expectedUrl)
            {
                string attachmentUrl = mUrlHelper.Kentico().ImageUrl(IMAGE_PATH, sizeConstraint);

                Assert.That(attachmentUrl, Is.EqualTo(expectedUrl));
            }

        }
    }
}
