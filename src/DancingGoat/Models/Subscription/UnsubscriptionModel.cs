using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DancingGoat.Models.Subscription
{
    public class UnsubscriptionModel
    {
        [Required]
        public string Email { get; set; }


        [Required]
        public Guid NewsletterGuid { get; set; }


        [Required]
        public Guid IssueGuid { get; set; }


        [Required]
        public string Hash { get; set; }


        public bool UnsubscribeFromAll { get; set; }


        [Bindable(false)]
        public string UnsubscriptionResult { get; set; }
    }
}