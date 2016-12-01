using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Kentico.Membership;

namespace DancingGoat.Models.Account
{
    public class RegisterViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "DancingGoatMvc.Register.EmailUserName.Empty")]
        [DisplayName("DancingGoatMvc.Register.EmailUserName")]
        [EmailAddress(ErrorMessage = "DancingGoatMvc.General.InvalidEmail")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("DancingGoatMvc.Register.Password")]
        [Required(ErrorMessage = "DancingGoatMvc.Register.Password.Empty")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("DancingGoatMvc.Register.PasswordConfirmation")]
        [Required(ErrorMessage = "DancingGoatMvc.Register.PasswordConfirmation.Empty")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        [Compare("Password", ErrorMessage = "DancingGoatMvc.Register.PasswordConfirmation.Invalid")]
        public string PasswordConfirmation { get; set; }


        [DisplayName("DancingGoatMvc.Register.FirstName")]
        [Required(ErrorMessage = "DancingGoatMvc.Register.FirstName.Empty")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string FirstName { get; set; }


        [DisplayName("DancingGoatMvc.Register.LastName")]
        [Required(ErrorMessage = "DancingGoatMvc.Register.LastName.Empty")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string LastName { get; set; }
    }
}