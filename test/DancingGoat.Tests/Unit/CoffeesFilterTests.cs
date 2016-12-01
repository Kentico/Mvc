using System.Linq;

using CMS.Tests;

using DancingGoat.Models.Coffees;

using NUnit.Framework;


namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class CoffeesFilterTests : UnitTests
    {
        [Test]
        public void Load_FilterContainsCorrectOptions()
        {
            var filter = new CoffeeFilterViewModel();
            filter.Load();

            var washed = filter.ProcessingTypes.FirstOrDefault(checkbox => checkbox.Value == "Washed");
            var semiwashed = filter.ProcessingTypes.FirstOrDefault(checkbox => checkbox.Value == "Semiwashed");
            var natural = filter.ProcessingTypes.FirstOrDefault(checkbox => checkbox.Value == "Natural");

            CMSAssert.All(
                () => Assert.AreEqual(3, filter.ProcessingTypes.Length),
                () => Assert.IsNotNull(washed),
                () => Assert.IsNotNull(semiwashed),
                () => Assert.IsNotNull(natural)
            );
        }


        [Test]
        public void GetWhereCondition_EmptyViewMode_EmptyWhereCondition()
        {
            var filter = new CoffeeFilterViewModel();
            filter.Load();

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.IsEmpty(where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetUpDecaf_RestrictionInWhereCondition()
        {
            var filter = new CoffeeFilterViewModel
            {
                OnlyDecaf = true
            };
            filter.Load();

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("[CoffeeIsDecaf] = 1", where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetUpCoffeeProcessingType_RestrictionInWhereCondition()
        {
            var filter = new CoffeeFilterViewModel();
            filter.Load();

            filter.ProcessingTypes[0].IsChecked = true;
            filter.ProcessingTypes[1].IsChecked = false;
            filter.ProcessingTypes[2].IsChecked = true;

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("[CoffeeProcessing] IN (N'Washed', N'Natural')", where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetUpFullFilter_RestrictionInWhereCondition()
        {
            var filter = new CoffeeFilterViewModel();
            filter.Load();

            filter.ProcessingTypes[0].IsChecked = true;
            filter.ProcessingTypes[1].IsChecked = true;
            filter.ProcessingTypes[2].IsChecked = true;
            filter.OnlyDecaf = true;

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("[CoffeeIsDecaf] = 1 AND [CoffeeProcessing] IN (N'Washed', N'Semiwashed', N'Natural')", where.ToString(true))
            );
        }
    }
}
