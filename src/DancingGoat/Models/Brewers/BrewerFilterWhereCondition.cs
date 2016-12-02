using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Brewers
{
    public class BrewerFilterWhereCondition
    {
        private readonly BrewerFilterViewModel mBrewerFilterViewModel;


        public BrewerFilterWhereCondition(BrewerFilterViewModel brewerFilterViewModel)
        {
            mBrewerFilterViewModel = brewerFilterViewModel;
        }


        public WhereCondition GetWhereCondition()
        {
            return new WhereCondition()
                .And(GetManufacturerWhereCondition())
                .And(GetPriceWhereCondition())
                .And(GetStatusWhereCondition())
                .And(GetInStockWhereCondition());
        }


        private WhereCondition GetManufacturerWhereCondition()
        {
            var where = new WhereCondition();

            var selectedManufacturerIds = GetSelectedManufacturerIds();

            if (selectedManufacturerIds.Any())
            {
                where.WhereIn("SKUManufacturerID", selectedManufacturerIds);
            }

            return where;
        }


        private WhereCondition GetStatusWhereCondition()
        {
            var where = new WhereCondition();

            var selectedStatusIds = GetSelectedStatusIds();

            if (selectedStatusIds.Any())
            {
                where.WhereIn("SKUPublicStatusID", selectedStatusIds);
            }

            return where;
        }


        private WhereCondition GetInStockWhereCondition()
        {
            var where = new WhereCondition();

            if (mBrewerFilterViewModel.OnlyInStock)
            {
                where.WhereGreaterThan("SKUAvailableItems", 0).Or().WhereNull("SKUAvailableItems");
            }

            return where;
        }


        private WhereCondition GetPriceWhereCondition()
        {
            var where = new WhereCondition();

            foreach (var range in GetSelectedRanges())
            {
                where.Or(GetWhereConditionForPriceRange(range));
            }

            return where;
        }


        private IEnumerable<BrewerPriceRangesEnum> GetSelectedRanges()
        {
            return mBrewerFilterViewModel.Prices
                .Where(price => price.IsChecked)
                .Select(GetRangeFromSelectedValue);
        }


        private static BrewerPriceRangesEnum GetRangeFromSelectedValue(BrewersProductFilterCheckboxViewModel price)
        {
            return (BrewerPriceRangesEnum)price.Value;
        }


        private static WhereCondition GetWhereConditionForPriceRange(BrewerPriceRangesEnum range)
        {
            switch (range)
            {
                case BrewerPriceRangesEnum.ToFifty:
                    return GetPriceRangeWhereCondition(0, 50);

                case BrewerPriceRangesEnum.FromFiftyToTwoHundredFifty:
                    return GetPriceRangeWhereCondition(50, 250);

                case BrewerPriceRangesEnum.FromTwoHundredFiftyToFiveThousand:
                    return GetPriceRangeWhereCondition(250, 5000);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static WhereCondition GetPriceRangeWhereCondition(int from, int to)
        {
            return new WhereCondition()
                .WhereGreaterOrEquals("SKUPrice", from)
                .And().WhereLessThan("SKUPrice", to);
        }


        private List<int> GetSelectedStatusIds()
        {
            return mBrewerFilterViewModel.PublicStatuses
                .Where(x => x.IsChecked)
                .Select(x => x.Value)
                .ToList();
        }


        private List<int> GetSelectedManufacturerIds()
        {
            return mBrewerFilterViewModel.Manufacturers
                .Where(x => x.IsChecked)
                .Select(x => x.Value)
                .ToList();
        }
    }
}