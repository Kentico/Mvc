using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Coffees
{
    public class CoffeeViewModel : ITypedProductViewModel
    {
        public int Altitude { get; set; }


        public string Country { get; set; }


        public string Farm { get; set; }


        public bool IsDecaf { get; set; }


        public string Processing { get; set; }


        public string Variety { get; set; }


        public static CoffeeViewModel GetViewModel(Coffee coffee)
        {
            return new CoffeeViewModel
            {
                Altitude = coffee.Fields.Altitude,
                Country = coffee.Fields.Country,
                Farm = coffee.Fields.Farm,
                IsDecaf = coffee.Fields.IsDecaf,
                Processing = coffee.Fields.Processing,
                Variety = coffee.Fields.Variety
            };
        }
    }
}