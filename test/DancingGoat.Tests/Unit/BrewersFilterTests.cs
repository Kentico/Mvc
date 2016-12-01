using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;
using CMS.Tests;

using DancingGoat.Models.Brewers;
using DancingGoat.Repositories;

using NSubstitute;
using NUnit.Framework;

using Tests.DocumentEngine;


namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class BrewersFilterTests : UnitTests
    {
        private IBrewerRepository mRepository;

        [SetUp]
        public void SetUp()
        {
            Fake().DocumentType<Brewer>(Brewer.CLASS_NAME);
            FakeManufacturers();
            FakePublicStatuses();

            var brewers = MockBrewers();

            mRepository = Substitute.For<IBrewerRepository>();
            mRepository.GetBrewers(null).Returns(brewers);
        }


        [Test]
        public void Load_FilterContainsCorrectManufacturers()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            var manufacturer1 = filter.Manufacturers.FirstOrDefault(checkbox => checkbox.Value == 1);
            var manufacturer2 = filter.Manufacturers.FirstOrDefault(checkbox => checkbox.Value == 2);

            CMSAssert.All(
                () => Assert.AreEqual(2, filter.Manufacturers.Count),
                () => Assert.IsNotNull(manufacturer1),
                () => Assert.IsNotNull(manufacturer2)
            );
        }


        [Test]
        public void Load_FilterContainsCorrectPrices()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            var toFifty = filter.Prices.FirstOrDefault(checkbox =>
                (int)BrewerPriceRangesEnum.ToFifty == checkbox.Value);

            var fromTwoHundredFiftyToFiveThousand = filter.Prices.FirstOrDefault(checkbox =>
                (int)BrewerPriceRangesEnum.FromTwoHundredFiftyToFiveThousand == checkbox.Value);

            var fromFiftyToTwoHundredFifty = filter.Prices.FirstOrDefault(checkbox =>
                (int)BrewerPriceRangesEnum.FromFiftyToTwoHundredFifty == checkbox.Value);

            CMSAssert.All(
                () => Assert.AreEqual(3, filter.Prices.Count),
                () => Assert.IsNotNull(toFifty),
                () => Assert.IsNotNull(fromTwoHundredFiftyToFiveThousand),
                () => Assert.IsNotNull(fromFiftyToTwoHundredFifty)
                );
        }


        [Test]
        public void Load_FilterContainsCorrectPublicStatuses()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            var status1 = filter.Manufacturers.FirstOrDefault(checkbox => checkbox.Value == 1);
            var status2 = filter.Manufacturers.FirstOrDefault(checkbox => checkbox.Value == 2);

            CMSAssert.All(
                () => Assert.AreEqual(2, filter.PublicStatuses.Count),
                () => Assert.IsNotNull(status1),
                () => Assert.IsNotNull(status2)
            );
        }


        [Test]
        public void GetWhereCondition_EmptyViewModel_EmptyWhereCondition()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.IsEmpty(where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetUpOnlyInStock_CorrectRestrictionInWhereCondition()
        {
            var filter = new BrewerFilterViewModel(mRepository)
            {
                OnlyInStock = true
            };
            filter.Load();

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("([SKUAvailableItems] > 0 OR [SKUAvailableItems] IS NULL)", where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetupManufacturers_CorrectRestrictionInWhereCondition()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            filter.Manufacturers[0].IsChecked = true;
            filter.Manufacturers[1].IsChecked = true;

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("[SKUManufacturerID] IN (1, 2)", where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetupPrice_CorrectRestrictionInWhereCondition()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            filter.Prices[0].IsChecked = true;
            filter.Prices[1].IsChecked = false;
            filter.Prices[2].IsChecked = true;

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("(([SKUPrice] >= 0 AND [SKUPrice] < 50) OR ([SKUPrice] >= 250 AND [SKUPrice] < 5000))", where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetupStatuses_CorrectRestrictionInWhereCondition()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            filter.PublicStatuses[0].IsChecked = true;
            filter.PublicStatuses[1].IsChecked = true;

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("[SKUPublicStatusID] IN (1, 2)", where.ToString(true))
            );
        }


        [Test]
        public void GetWhereCondition_SetupFullFilter_ReturnsCorrectWhereCondition()
        {
            var filter = new BrewerFilterViewModel(mRepository);
            filter.Load();

            filter.Manufacturers[0].IsChecked = true;
            filter.Manufacturers[1].IsChecked = true;

            filter.Prices[0].IsChecked = true;
            filter.Prices[1].IsChecked = true;

            filter.PublicStatuses[0].IsChecked = true;
            filter.PublicStatuses[1].IsChecked = true;

            filter.OnlyInStock = true;

            var where = filter.GetWhereCondition();

            CMSAssert.All(
                () => Assert.IsNotNull(where),
                () => Assert.AreEqual("[SKUManufacturerID] IN (1, 2) AND (([SKUPrice] >= 0 AND [SKUPrice] < 50) OR ([SKUPrice] >= 50 AND [SKUPrice] < 250)) AND [SKUPublicStatusID] IN (1, 2) AND ([SKUAvailableItems] > 0 OR [SKUAvailableItems] IS NULL)", where.ToString(true))
            );
        }


        private void FakePublicStatuses()
        {
            Fake<PublicStatusInfo, PublicStatusInfoProvider>().WithData(
                new PublicStatusInfo
                {
                    PublicStatusID = 1,
                    PublicStatusDisplayName = "Status1"
                },
                new PublicStatusInfo
                {
                    PublicStatusID = 2,
                    PublicStatusDisplayName = "Status2"
                }
            );
        }


        private void FakeManufacturers()
        {
            Fake<ManufacturerInfo, ManufacturerInfoProvider>().WithData(
                new ManufacturerInfo
                {
                    ManufacturerID = 1,
                    ManufacturerDisplayName = "Manufacturer1",
                    ManufacturerName = "Manufacturer1"
                },
                new ManufacturerInfo
                {
                    ManufacturerID = 2,
                    ManufacturerDisplayName = "Manufacturer2",
                    ManufacturerName = "Manufacturer2"
                }
            );
        }


        private List<Brewer> MockBrewers()
        {
            var skuInfo1 = Substitute.For<SKUInfo>();
            var skuInfo2 = Substitute.For<SKUInfo>();
            skuInfo1.SKUManufacturerID.Returns(1);
            skuInfo1.SKUPublicStatusID.Returns(1);
            skuInfo2.SKUManufacturerID.Returns(2);
            skuInfo2.SKUPublicStatusID.Returns(2);

            var brewer1 = TreeNode.New<Brewer>().With(x =>
            {
                x.DocumentName = "Brewer1";
                x.SKU = skuInfo1;
            });
            var brewer2 = TreeNode.New<Brewer>().With(x =>
            {
                x.DocumentName = "Brewer2";
                x.SKU = skuInfo2;
            });

            return new List<Brewer> { brewer1, brewer2 };
        }
    }
}