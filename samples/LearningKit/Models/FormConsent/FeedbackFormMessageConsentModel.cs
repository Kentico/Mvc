using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LearningKit.Models.Form
{
    public class FeedbackFormMessageConsentModel
    {
        [DisplayName("Name")]
        [DataType(DataType.Text)]
        [MaxLength(200, ErrorMessage = "The name cannot be longer than 200 characters.")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [DataType(DataType.Text)]
        [MaxLength(200, ErrorMessage = "The last name cannot be longer than 200 characters.")]
        public string LastName { get; set; }

        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The email address field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(254, ErrorMessage = "The email address cannot be longer than 254 characters.")]
        public string Email { get; set; }

        [DisplayName("Message")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "The message field is required.")]
        [MaxLength(2000, ErrorMessage = "The message cannot be longer than 2000 characters.")]
        public string MessageText { get; set; }

        //DocSection:Consent
        public string ConsentShortText { get; set; }

        public bool ConsentIsAgreed { get; set; }
        //EndDocSection:Consent
    }
}