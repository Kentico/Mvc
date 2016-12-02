using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Kentico.Membership;

namespace DancingGoat.Models.Account
{
    public class PersonalDetailsViewModel
    {
        [DisplayName("DancingGoatMvc.SignIn.EmailUserName")]
        public string UserName { get; set; }


        [DisplayName("DancingGoatMvc.Register.FirstName")]
        [Required(ErrorMessage = "DancingGoatMvc.Register.FirstName.Empty")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "DancingGoatMvc.Register.LastName.Empty")]
        [DisplayName("DancingGoatMvc.Register.LastName")]
        [MaxLength(100, ErrorMessage = @"DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string LastName { get; set; }


        public PersonalDetailsViewModel()
        {
            
        }


        public PersonalDetailsViewModel(User user)
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
    }
}