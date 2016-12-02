using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "DancingGoatMvc.SignIn.EmailUserName.Empty")]
        [DisplayName("DancingGoatMvc.SignIn.EmailUserName")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("DancingGoatMvc.SignIn.Password")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string Password { get; set; }


        [DisplayName("DancingGoatMvc.SignIn.StaySignedIn")]
        public bool StaySignedIn { get; set; }
    }
}