using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Checkout
{
    public class CartItemUpdateModel
    {
        public int ID { get; set; }


        public int SKUID { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "DancingGoatMvc.ShoppingCart.UnitsGreaterThanZero")]
        public int Units { get; set; }
    }
}