using System.Collections.Generic;

using DancingGoat.Repositories.Filters;

namespace DancingGoat.Models.Products
{
    public class ProductListViewModel
    {
        public IRepositoryFilter Filter { get; set; }

        public IEnumerable<ProductListItemViewModel> Items { get; set; }
    }
}