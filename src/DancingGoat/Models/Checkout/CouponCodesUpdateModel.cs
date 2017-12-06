using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Checkout
{
    public class CouponCodesUpdateModel
    {
        [MaxLength(200, ErrorMessage = "General.MaxlengthExceeded")]
        public string NewCouponCode { get; set; }


        public string RemoveCouponCode { get; set; }
    }
}