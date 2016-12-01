using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Contacts
{
    public class MessageModel
    {
        [Display(Name = "General.Firstname")]
        [DataType(DataType.Text)]
        [MaxLength(200, ErrorMessage = "General.MaxlengthExceeded")]
        public string FirstName { get; set; }


        [Display(Name = "General.Lastname")]
        [DataType(DataType.Text)]
        [MaxLength(200, ErrorMessage = "General.MaxlengthExceeded")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "General.RequireEmail")]
        [Display(Name = "General.EmailAddress")]
        [EmailAddress(ErrorMessage = "General.CorrectEmailFormat")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.News.LongEmail")]
        public string Email { get; set; }


        [Required(ErrorMessage = "General.RequiresMessage")]
        [Display(Name = "General.Message")]
        [DataType(DataType.MultilineText)]
        [MaxLength(500, ErrorMessage = "General.MaxlengthExceeded")]
        public string MessageText { get; set; }
    }
}