using System.Collections.Generic;

using LearningKit.Models.Products;

namespace LearningKit.Models.ProductFilter
{
    public class ProductFilterViewModel
    {
        public List<ProductFilterCheckboxViewModel> Manufacturers { get; set; }

        public bool LPTWithFeature { get; set; }

        public decimal PriceFrom { get; set; }

        public decimal PriceTo { get; set; }

        public List<ProductListItemViewModel> FilteredProducts { get; set; }
    }
}