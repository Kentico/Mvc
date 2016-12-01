using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Subscription
{
    public class ConfirmSubscriptionModel
    {
        [Required]
        public string SubscriptionHash { get; set; }


        public string DateTime { get; set; }


        [Bindable(false)]
        public string ConfirmationResult { get; set; }
    }
}