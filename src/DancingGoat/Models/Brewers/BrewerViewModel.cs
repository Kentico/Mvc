using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Brewers
{
    public class BrewerViewModel : ITypedProductViewModel
    {
        public bool IsDishwasherSafe { get; set; }


        public static BrewerViewModel GetViewModel(Brewer brewer)
        {
            return new BrewerViewModel
            {
                IsDishwasherSafe = brewer.Fields.IsDishwasherSafe
            };
        }
    }
}