using System;

using CMS.Tests;

using NUnit.Framework;

namespace Kentico.Content.Web.Mvc.Tests
{
    /// <summary>
    /// Unit tests for class SizeConstraint.
    /// </summary>
    public class SizeConstraintTests
    {
        [TestFixture]
        [Category("Unit")]
        public class IsEmptyTests
        {
            [Test]
            public void IsEmpty_EmptySizeConstraint_ReturnsTrue()
            {
                var constraint = SizeConstraint.Empty;
                Assert.That(constraint.IsEmpty, Is.True);
            }


            [Test]
            public void IsEmpty_SizeConstraint_ReturnsFalse()
            {
                var constraint = SizeConstraint.Size(100, 200);
                Assert.That(constraint.IsEmpty, Is.False);
            }


            [Test]
            public void IsEmpty_MaxWidthOrHeightConstraint_ReturnsFalse()
            {
                var constraint = SizeConstraint.MaxWidthOrHeight(200);
                Assert.That(constraint.IsEmpty, Is.False);
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class SizeTests
        {
            [Test]
            public void Size_ValidBothArguments_SetsWidthAndHeightProperties()
            {
                var constraint = SizeConstraint.Size(100, 200);

                CMSAssert.All(
                    () => Assert.That(constraint.WidthComponent, Is.EqualTo(100)),
                    () => Assert.That(constraint.HeightComponent, Is.EqualTo(200)),
                    () => Assert.That(constraint.MaxWidthOrHeightComponent, Is.EqualTo(0)));
            }


            [TestCase(-100, 200)]
            [TestCase(0, 200)]
            public void Size_InvalidWidthArgument_ThrowsException(int width, int height)
            {
                Assert.That(() => SizeConstraint.Size(width, height),
                    Throws.Exception
                        .TypeOf<ArgumentOutOfRangeException>()
                        .With
                        .Property("ParamName")
                        .EqualTo("width"));
            }


            [TestCase(100, -200)]
            [TestCase(100, 0)]
            public void Size_InvalidHeighArgument_ThrowsException(int width, int height)
            {
                Assert.That(() => SizeConstraint.Size(width, height),
                    Throws.Exception
                        .TypeOf<ArgumentOutOfRangeException>()
                        .With
                        .Property("ParamName")
                        .EqualTo("height"));
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class WidthTests
        {
            [Test]
            public void Width_ValidArgument_SetsWidthComponentProperty()
            {
                var constraint = SizeConstraint.Width(100);

                CMSAssert.All(
                    () => Assert.That(constraint.WidthComponent, Is.EqualTo(100)),
                    () => Assert.That(constraint.HeightComponent, Is.EqualTo(0)),
                    () => Assert.That(constraint.MaxWidthOrHeightComponent, Is.EqualTo(0)));
            }


            [TestCase(-200)]
            [TestCase(0)]
            public void Width_InvalidArgument_ThrowsException(int width)
            {
                Assert.That(() => SizeConstraint.Width(width),
                    Throws.Exception
                        .TypeOf<ArgumentOutOfRangeException>()
                        .With
                        .Property("ParamName")
                        .EqualTo("width"));
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class HeightTests
        {
            [Test]
            public void Height_ValidArgument_SetsHeightComponentProperty()
            {
                var constraint = SizeConstraint.Height(100);

                CMSAssert.All(
                    () => Assert.That(constraint.HeightComponent, Is.EqualTo(100)),
                    () => Assert.That(constraint.WidthComponent, Is.EqualTo(0)),
                    () => Assert.That(constraint.MaxWidthOrHeightComponent, Is.EqualTo(0)));
            }


            [TestCase(-200)]
            [TestCase(0)]
            public void Height_InvalidPramater_ThrowsException(int height)
            {
                Assert.That(() => SizeConstraint.Height(height),
                    Throws.Exception
                        .TypeOf<ArgumentOutOfRangeException>()
                        .With
                        .Property("ParamName")
                        .EqualTo("height"));
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class MaxWidthOrHeightTests
        {
            [Test]
            public void MaxWidthOrHeight_ValidArgument_SetsMaxWidthOrHeightComponentProperty()
            {
                var constraint = SizeConstraint.MaxWidthOrHeight(100);

                CMSAssert.All(
                    () => Assert.That(constraint.HeightComponent, Is.EqualTo(0)),
                    () => Assert.That(constraint.WidthComponent, Is.EqualTo(0)),
                    () => Assert.That(constraint.MaxWidthOrHeightComponent, Is.EqualTo(100)));
            }


            [TestCase(-200)]
            [TestCase(0)]
            public void MaxWidthOrHeight_InvalidArgument_ThrowsException(int maxWidthOrHeight)
            {
                Assert.That(() => SizeConstraint.MaxWidthOrHeight(maxWidthOrHeight),
                    Throws.Exception
                        .TypeOf<ArgumentOutOfRangeException>()
                        .With
                        .Property("ParamName")
                        .EqualTo("maxWidthOrHeight"));
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class EqualOperatorTests
        {
            [Test]
            public void EqualOperator_EmptyConstraint_ReturnsTrue()
            {
                var constraint1 = SizeConstraint.Empty;
                var constraint2 = SizeConstraint.Empty;

                Assert.That(constraint1 == constraint2, Is.True);
            }


            [Test]
            public void EqualOperator_DifferentInstancesSameSizes_ReturnsTrue()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);
                Assert.That(constraint1 == constraint2, Is.True);
            }


            [Test]
            public void EqualOperator_DifferentInstancesDifferentSizes_ReturnsFalse()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(300, 500);
                Assert.That(constraint1 == constraint2, Is.False);
            }


            [Test]
            public void EqualOperator_CommutativeProperty_SameResults()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);

                CMSAssert.All(
                    () => Assert.That(constraint1 == constraint2, Is.True),
                    () => Assert.That(constraint2 == constraint1, Is.True));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class NotEqualOperatorTests
        {
            [Test]
            public void NotEqualOperator_EmptyConstraint_ReturnsFalse()
            {
                var constraint1 = SizeConstraint.Empty;
                var constraint2 = SizeConstraint.Empty;

                Assert.That(constraint1 != constraint2, Is.False);
            }


            [Test]
            public void NotEqualOperator_DifferentInstancesSameSizes_ReturnsFalse()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);
                Assert.That(constraint1 != constraint2, Is.False);
            }


            [Test]
            public void NotEqualOperator_DifferentInstancesDifferentSizes_ReturnsTrue()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(300, 500);
                Assert.That(constraint1 != constraint2, Is.True);
            }


            [Test]
            public void NotEqualOperator_CommutativeProperty_SameResults()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);

                CMSAssert.All(
                    () => Assert.That(constraint1 != constraint2, Is.False),
                    () => Assert.That(constraint2 != constraint1, Is.False));
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class EqualsTests
        {
            [Test]
            public void Equals_EmptyConstraint_ReturnsTrue()
            {
                var constraint1 = SizeConstraint.Empty;
                var constraint2 = SizeConstraint.Empty;

                Assert.That(constraint1.Equals(constraint2), Is.True);
            }


            [Test]
            public void Equals_DifferentInstancesSameSizes_ReturnsTrue()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);
                Assert.That(constraint1.Equals(constraint2), Is.True);
            }


            [Test]
            public void Equals_DifferentInstancesDifferentSizes_ReturnsFalse()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(300, 500);
                Assert.That(constraint1.Equals(constraint2), Is.False);
            }


            [Test]
            public void Equals_CommutativeProperty_SameResults()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);

                CMSAssert.All(
                    () => Assert.That(constraint1.Equals(constraint2), Is.True),
                    () => Assert.That(constraint2.Equals(constraint1), Is.True));
            }
        }


        [TestFixture]
        [Category("Unit")]
        public class GetHashCodeTests
        {
            [Test]
            public void GetHashCode_EmptyConstraint_SameHashCode()
            {
                var constraint1 = SizeConstraint.Empty;
                var constraint2 = SizeConstraint.Empty;
                Assert.AreEqual(constraint1.GetHashCode(), constraint2.GetHashCode());
            }


            [Test]
            public void GetHashCode_SameSizes_SameHashCode()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(100, 200);
                Assert.AreEqual(constraint1.GetHashCode(), constraint2.GetHashCode());
            }


            [Test]
            public void GetHashCode_DifferentSizes_DifferentHashCode()
            {
                var constraint1 = SizeConstraint.Size(100, 200);
                var constraint2 = SizeConstraint.Size(101, 200);
                Assert.AreNotEqual(constraint1.GetHashCode(), constraint2.GetHashCode());
            }


            [TestCase(1, 1)]
            [TestCase(1000, 1000)]
            [TestCase(Int32.MaxValue, Int32.MaxValue)]
            public void GetHashCode_SameWidth_SameHashCode(int firstConstraintWidth, int secondConstraintWidth)
            {
                var constraint1 = SizeConstraint.Width(firstConstraintWidth);
                var constraint2 = SizeConstraint.Width(secondConstraintWidth);
                Assert.AreEqual(constraint1, constraint2);
            }


            [TestCase(1, 2)]
            [TestCase(1, 100)]
            [TestCase(1000, 1001)]
            [TestCase(1, Int32.MaxValue)]
            public void GetHashCode_DifferentWidth_DifferentHashCode(int firstConstraintWidth, int secondConstraintWidth)
            {
                var constraint1 = SizeConstraint.Width(firstConstraintWidth);
                var constraint2 = SizeConstraint.Width(secondConstraintWidth);
                Assert.AreNotEqual(constraint1, constraint2);
            }


            [TestCase(1, 1)]
            [TestCase(1000, 1000)]
            [TestCase(Int32.MaxValue, Int32.MaxValue)]
            public void GetHashCode_SameHeight_SameHashCode(int firstConstraintWidth, int secondConstraintWidth)
            {
                var constraint1 = SizeConstraint.Height(firstConstraintWidth);
                var constraint2 = SizeConstraint.Height(secondConstraintWidth);
                Assert.AreEqual(constraint1, constraint2);
            }


            [TestCase(1, 2)]
            [TestCase(1, 100)]
            [TestCase(1000, 1001)]
            [TestCase(1, Int32.MaxValue)]
            public void GetHashCode_DifferentHeight_DifferentHashCode(int firstConstraintWidth, int secondConstraintWidth)
            {
                var constraint1 = SizeConstraint.Height(firstConstraintWidth);
                var constraint2 = SizeConstraint.Height(secondConstraintWidth);
                Assert.AreNotEqual(constraint1, constraint2);
            }


            [TestCase(1, 1)]
            [TestCase(1000, 1000)]
            [TestCase(Int32.MaxValue, Int32.MaxValue)]
            public void GetHashCode_SameMaxWidthOrHeight_SameHashCode(int firstConstraintWidth, int secondConstraintWidth)
            {
                var constraint1 = SizeConstraint.MaxWidthOrHeight(firstConstraintWidth);
                var constraint2 = SizeConstraint.MaxWidthOrHeight(secondConstraintWidth);
                Assert.AreEqual(constraint1, constraint2);
            }


            [TestCase(1, 2)]
            [TestCase(1, 100)]
            [TestCase(1000, 1001)]
            [TestCase(1, Int32.MaxValue)]
            public void GetHashCode_DifferentMaxWidthOrHeight_DifferentHashCode(int firstConstraintWidth, int secondConstraintWidth)
            {
                var constraint1 = SizeConstraint.MaxWidthOrHeight(firstConstraintWidth);
                var constraint2 = SizeConstraint.MaxWidthOrHeight(secondConstraintWidth);
                Assert.AreNotEqual(constraint1, constraint2);
            }
        }
    }
}
