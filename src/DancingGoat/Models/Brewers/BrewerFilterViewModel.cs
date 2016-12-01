using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using CMS.DataEngine;
using DancingGoat.Repositories;
using DancingGoat.Repositories.Filters;

namespace DancingGoat.Models.Brewers
{
    public class BrewerFilterViewModel : IRepositoryFilter
    {
        private readonly IBrewerRepository mBrewerRepository;

        [UIHint("BrewersProductFilter")]
        public List<BrewersProductFilterCheckboxViewModel> Manufacturers { get; set; }

        [UIHint("BrewersProductFilter")]
        public List<BrewersProductFilterCheckboxViewModel> Prices { get; set; }

        [UIHint("BrewersProductFilter")]
        public List<BrewersProductFilterCheckboxViewModel> PublicStatuses { get; set; }


        public bool OnlyInStock { get; set; }


        public BrewerFilterViewModel()
        {
        }


        public BrewerFilterViewModel(IBrewerRepository repository)
            : this()
        {
            mBrewerRepository = repository;
        }


        public void Load()
        {
            Manufacturers = GetManufacturers();
            Prices = GetPrices();
            PublicStatuses = GetPublicStatuses();
        }


        public string GetCacheKey()
        {
            return new BrewerFilterCacheKey(this).GetCacheKey();
        }


        public WhereCondition GetWhereCondition()
        {
            return new BrewerFilterWhereCondition(this).GetWhereCondition();
        }


        private List<BrewersProductFilterCheckboxViewModel> GetManufacturers()
        {
            var manufacturers = mBrewerRepository.GetBrewers(null)
                 .Where(brewer => brewer.Product.Manufacturer != null)
                 .Select(brewer =>
                     new
                     {
                         brewer.Product.Manufacturer?.ManufacturerID,
                         brewer.Product.Manufacturer?.ManufacturerDisplayName
                     })
                 .Distinct();

            return manufacturers.Select(manufacturer => new BrewersProductFilterCheckboxViewModel
            {
                DisplayName = manufacturer.ManufacturerDisplayName,
                Value = (int)manufacturer.ManufacturerID
            }).ToList();
        }


        private List<BrewersProductFilterCheckboxViewModel> GetPrices()
        {
            return new List<BrewersProductFilterCheckboxViewModel>
            {
                new BrewersProductFilterCheckboxViewModel {DisplayName = "$0 - $50", Value = (int)BrewerPriceRangesEnum.ToFifty},
                new BrewersProductFilterCheckboxViewModel {DisplayName = "$50 - $250", Value = (int)BrewerPriceRangesEnum.FromFiftyToTwoHundredFifty},
                new BrewersProductFilterCheckboxViewModel {DisplayName = "$250 - $5000", Value = (int)BrewerPriceRangesEnum.FromTwoHundredFiftyToFiveThousand}
            };
        }


        private List<BrewersProductFilterCheckboxViewModel> GetPublicStatuses()
        {
            var statuses = mBrewerRepository.GetBrewers(null)
                .Where(brewer => brewer.Product.PublicStatus != null)
                .Select(brewer =>
                    new
                    {
                        brewer.Product.PublicStatus?.PublicStatusID,
                        brewer.Product.PublicStatus?.PublicStatusDisplayName
                    })
                .Distinct();

            return statuses.Select(status => new BrewersProductFilterCheckboxViewModel
            {
                DisplayName = status.PublicStatusDisplayName,
                Value = (int)status.PublicStatusID
            }).ToList();
        }
    }
}