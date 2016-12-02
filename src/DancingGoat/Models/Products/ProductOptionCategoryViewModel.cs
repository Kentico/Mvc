using System.Collections.Generic;

using CMS.Ecommerce;

using Kentico.Ecommerce;

namespace DancingGoat.Models.Products
{
    public class ProductOptionCategoryViewModel
    {
        private readonly ProductOptionCategory Category;


        public string Name => Category.DisplayName;


        public OptionCategorySelectionTypeEnum SelectionType => Category.SelectionType;


        public IEnumerable<SKUInfo> Options => Category.CategoryOptions;


        public int SelectedOptionId { get; set; }


        public ProductOptionCategoryViewModel(int selectedOptionID, ProductOptionCategory category)
        {
            SelectedOptionId = selectedOptionID;
            Category = category;
        }
    }
}