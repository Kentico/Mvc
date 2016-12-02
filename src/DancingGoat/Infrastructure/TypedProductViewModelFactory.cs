using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;

using DancingGoat.Models.Brewers;
using DancingGoat.Models.Coffees;
using DancingGoat.Models.Products;

namespace DancingGoat.Infrastructure
{
    public class TypedProductViewModelFactory
    {
        /// <summary>
        /// Creates a product view model according to the product runtime type.
        /// </summary>
        /// <param name="product">Product node.</param>
        /// <returns>Strongly typed view model. Returns <c>null</c> if the specified product has no view model associated.</returns>
        public ITypedProductViewModel GetViewModel(SKUTreeNode product)
        {
            // Use dynamic dispatch to create the view model according to the product runtime type
            return CreateViewModel((dynamic)product);
        }


        private ITypedProductViewModel CreateViewModel(SKUTreeNode product)
        {
            return null;
        }


        private BrewerViewModel CreateViewModel(Brewer brewer)
        {
            return BrewerViewModel.GetViewModel(brewer);
        }


        private CoffeeViewModel CreateViewModel(Coffee coffee)
        {
            return CoffeeViewModel.GetViewModel(coffee);
        }
    }
}