using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DancingGoat.Models.Account
{
    public class RetrievePasswordViewModel
    {
        [Required(ErrorMessage = "DancingGoatMvc.PasswordReset.Email.Empty")]
        [DisplayName("DancingGoatMvc.PasswordReset.Email")]
        [EmailAddress(ErrorMessage = "DancingGoatMvc.General.InvalidEmail")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string Email { get; set; }
    }
}