using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using NUnit.Framework;

namespace Kentico.Content.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class <see cref="NameValueCollectionExtensions"/>.
    /// </summary>
    public class NameValueCollectionExtensionsTests
    {
        [TestFixture]
        [Category("Unit")]
        public class AddSizeConstraintTests
        {
            public class AddSizeConstraintTestCases : IEnumerable<TestCaseData>
            {
                public IEnumerator<TestCaseData> GetEnumerator()
                {
                    yield return new TestCaseData(SizeConstraint.Empty, "").SetName("MediaFile_EmptySizeConstraint_UrlWithoutQueryString.");
                    yield return new TestCaseData(SizeConstraint.Size(100, 200), $"?width={100}&height={200}&resizemode=force").SetName("AppendSizeConstraint_BothWidthAndHeight_QueryStringContainsBothWidthAndHeightParam");
                    yield return new TestCaseData(SizeConstraint.Width(300), $"?width={300}&resizemode=force").SetName("AppendSizeConstraint_WidthOnly_QueryStringContainsWidthParam");
                    yield return new TestCaseData(SizeConstraint.Height(500), $"?height={500}&resizemode=force").SetName("AppendSizeConstraint_HeightOnly_QueryStringContainsHeightParam");
                    yield return new TestCaseData(SizeConstraint.MaxWidthOrHeight(600), $"?maxsidesize={600}&resizemode=force").SetName("AppendSizeConstraint_MaxWidthOrHeight_QueryStringContainsMaxSideSizeParam");
                }


                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }


            [Test]
            public void Append_DifferentTypes_CorrectQueryString()
            {
                var sizeConstraintParameters = new NameValueCollection
                {
                    {"name", "John"},
                    {"age", 31},
                    {"exactSearch", false}
                };

                Assert.That(sizeConstraintParameters.ToQueryString(), Is.EqualTo("?name=John&age=31&exactSearch=False"));
            }


            [Test, TestCaseSource(typeof(AddSizeConstraintTestCases))]
            public void AddSizeConstraint_DifferentSizeConstraints_CorrectQueryString(SizeConstraint sizeConstraint, string expectedQueryString)
            {
                var sizeConstraintParameters = new NameValueCollection();
                sizeConstraintParameters.AddSizeConstraint(sizeConstraint);

                Assert.That(sizeConstraintParameters.ToQueryString(), Is.EqualTo(expectedQueryString));
            }
        }
    }
}
