using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Subscription
{
    public class SubscribeModel
    {
        [Required(ErrorMessage = "General.RequireEmail")]
        [EmailAddress(ErrorMessage = "General.CorrectEmailFormat")]
        [DisplayName("DancingGoatMvc.News.SubscriberEmail")]
        [MaxLength(250, ErrorMessage = "DancingGoatMvc.News.LongEmail")]
        public string Email { get; set; }
    }
}