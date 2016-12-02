using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using DancingGoat.Repositories.Filters;

namespace DancingGoat.Models.Coffees
{
    public class CoffeeFilterViewModel : IRepositoryFilter
    {
        public bool OnlyDecaf { get; set; }


        [UIHint("CoffeeProductFilter")]
        public CoffeeProductFilterCheckboxViewModel[] ProcessingTypes { get; set; }


        public void Load()
        {
            ProcessingTypes = GetCoffeeProcessingTypes()
               .Select(GetProductFilterCheckboxViewModel)
               .ToArray();
        }


        public WhereCondition GetWhereCondition()
        {
            var decafWhereCondition = GetDecafWhereCondition();
            var processingTypesWhere = GetProcessingTypesWhere();

            return decafWhereCondition.And(processingTypesWhere);
        }


        public string GetCacheKey()
        {
            var serializedProcessingTypes = ProcessingTypes
               .Select(type => string.Format($"{type.Value}:{type.IsChecked}"))
               .Join(TextHelper.NewLine);

            return string.Format($"OnlyDecaf:{OnlyDecaf}{TextHelper.NewLine}{serializedProcessingTypes}");
        }


        private WhereCondition GetDecafWhereCondition()
        {
            var whereCondition = new WhereCondition();

            if (OnlyDecaf)
            {
                whereCondition.WhereTrue("CoffeeIsDecaf");
            }
            return whereCondition;
        }


        private WhereCondition GetProcessingTypesWhere()
        {
            var whereCondition = new WhereCondition();
            var selectedTypes = GetSelectedTypes();

            if (selectedTypes.Any())
            {
                whereCondition.WhereIn("CoffeeProcessing", selectedTypes);
            }

            return whereCondition;

        }


        private List<string> GetSelectedTypes()
        {
            return ProcessingTypes
                .Where(x => x.IsChecked)
                .Select(x => x.Value)
                .ToList();
        }


        private IEnumerable<string> GetCoffeeProcessingTypes()
        {
            return new List<string> { "Washed", "Semiwashed", "Natural" };
        }


        private static CoffeeProductFilterCheckboxViewModel GetProductFilterCheckboxViewModel(string processingType)
        {
            return new CoffeeProductFilterCheckboxViewModel
            {
                DisplayName = ResHelper.GetString("DancingGoatMvc.Coffees." + processingType),
                Value = processingType
            };
        }
    }
}