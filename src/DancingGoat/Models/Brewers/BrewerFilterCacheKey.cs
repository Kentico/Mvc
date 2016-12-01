using System.Collections.Generic;
using System.Linq;

using CMS.Helpers;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Brewers
{
    internal class BrewerFilterCacheKey
    {
        private readonly BrewerFilterViewModel mBrewerFilterViewModel;


        public BrewerFilterCacheKey(BrewerFilterViewModel brewerFilterViewModel)
        {
            mBrewerFilterViewModel = brewerFilterViewModel;
        }


        public string GetCacheKey()
        {
            var manufacturers = GetCacheKeyForCollection(mBrewerFilterViewModel.Manufacturers);
            var prices = GetCacheKeyForCollection(mBrewerFilterViewModel.Prices);
            var statuses = GetCacheKeyForCollection(mBrewerFilterViewModel.PublicStatuses);

            return string.Format($"OnlyInStock:{mBrewerFilterViewModel.OnlyInStock}{TextHelper.NewLine}" +
                                 $"{manufacturers}{TextHelper.NewLine}" +
                                 $"{prices}{TextHelper.NewLine}" +
                                 $"{statuses}");
        }


        private string GetCacheKeyForCollection(IEnumerable<BrewersProductFilterCheckboxViewModel> checkboxViewModels)
        {
            return checkboxViewModels
                .Select(type => string.Format($"{type.Value}:{type.IsChecked}"))
                .Join(TextHelper.NewLine);
        }
    }
}