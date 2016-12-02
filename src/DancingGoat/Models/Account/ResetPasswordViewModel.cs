using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DancingGoat.Models.Account
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }


        public string Token { get; set; }


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
    }
}