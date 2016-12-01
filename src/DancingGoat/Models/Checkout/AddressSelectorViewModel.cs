using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace DancingGoat.Models.Checkout
{
    public class AddressSelectorViewModel
    {
        [Display(Name = "DancingGoatMvc.Address")]
        public int? AddressID { get; set; }
        

        public SelectList Addresses { get; set; }


        public bool HasSomeAddress => (Addresses != null) && Addresses.Any();
    }
}