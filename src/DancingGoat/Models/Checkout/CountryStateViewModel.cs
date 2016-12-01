using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "Countries")]
    public class CountryStateViewModel
    {
        [Required(ErrorMessage = "DancingGoatMvc.Address.CountryIsRequired")]
        [Display(Name = "DancingGoatMvc.Address.Country")]
        public int CountryID { get; set; }


        [Display(Name = "DancingGoatMvc.Address.State")]
        [RegularExpression(@"[0-9]*$", ErrorMessage = "DancingGoatMvc.Address.StateIsRequired")]
        public int? StateID { get; set; }


        public SelectList Countries { get; set; }
    }
}